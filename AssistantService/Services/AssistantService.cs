using System.Collections.Concurrent;
using System.Reflection;
using AssistantService.Models;
using AssistantService.Utilities;
using AutoMapper;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;
using Twilio.TwiML;

namespace AssistantService.Services;

public class AssistantServiceClass : IAssistantService
{
	private readonly ITwilioService _twilioService;
	private readonly IChatGPTService _chatService;
	private readonly ICacheService _cacheService;

	private readonly ILogger<AssistantServiceClass> _logger;
	private readonly IMapper _mapper;

	private readonly CallHistories.CallHistoriesClient _callHistoryClient;
	private readonly Users.UsersClient _usersClient;

	private readonly ChatToolsFunctions _chatToolsFunctions;

	public AssistantServiceClass(
		ITwilioService twilioService,
		IChatGPTService chatService,
		ICacheService cacheService,
		ILogger<AssistantServiceClass> logger,
		IMapper mapper,
		ChatToolsFunctions chatToolsFunctions,
		CallHistories.CallHistoriesClient callHistoryClient,
		Users.UsersClient usersClient
	)
	{
		_twilioService = twilioService;
		_chatService = chatService;
		_cacheService = cacheService;
		_logger = logger;
		_mapper = mapper;
		_chatToolsFunctions = chatToolsFunctions;
		_callHistoryClient = callHistoryClient;
		_usersClient = usersClient;
	}

	public async Task<VoiceResponse> ProcessIncomingRequest(TwilioInputForm input)
	{
		var request = new GetUserByPhoneNumberRequest { PhoneNumber = input.From };

		var reply = await _usersClient.GetUserByPhoneNumberAsync(request);
		User? user = _mapper.Map<User>(reply.UserEntity);
		VoiceResponse errorResponse = _twilioService.ErrorResponse();

		if (user == null)
		{
			_logger.LogError("User not found");
			return errorResponse;
		}
		if (input.CallStatus == "ringing")
		{
			return ProccessInitialCall(input, user);
		}
		else if (input.CallStatus == "in-progress")
		{
			return await ProcessInProgressRequests(input, user);
		}
		else if (input.CallStatus == "completed")
		{
			return await ProccessCompletedCall(input, user);
		}
		_logger.LogError("Invalid call status");
		return errorResponse;
	}

	public VoiceResponse ProccessInitialCall(TwilioInputForm input, User user)
	{
		DateTime currentDateTime = DateTime.Now;
		string initialMessage =
			$"You are an intelligent chatbot integrated with a call system. Your responses are based on real-time transcripts provided by Twilio's machine learning transcription service. Each segment of the conversation will be separated by '---'. You have access to advanced functions to enhance your responses and provide assistance as needed. Ensure clarity, accuracy, and natural interaction when responding to these transcripts. When responding, use the current date {currentDateTime} as a reference point unless the user specifies otherwise. Always return a message especially with tool calls.";

		_cacheService.CreateConversationHistory(
			input.CallSid,
			input.CallStatus,
			input.From,
			input.To,
			user.UserID,
			initialMessage,
			user
		);
		VoiceResponse intialResponse = _twilioService.CallResponse(
			"Welcome I am GPT your assistant, what would you like to do?"
		);
		return intialResponse;
	}

	public async Task<VoiceResponse> ProcessInProgressRequests(TwilioInputForm input, User user)
	{
		if (input.SpeechResult == null)
		{
			input.SpeechResult = "No speech detected";
		}
		_cacheService.UpdateConversationHistory(input.CallSid, input.SpeechResult);

		List<string>? convoHistory = _cacheService.GetConvoHistory(input.CallSid);

		string messages = string.Join("---", convoHistory ?? new List<string>());

		ChatCompletionData GPTresponse = await _chatService.GetChatCompletion(messages);

		string? message = GPTresponse.Message;

		if (message != null)
		{
			_cacheService.UpdateConversationHistory(input.CallSid, message);
		}

		IReadOnlyList<ChatToolCall>? tools = GPTresponse.ToolCalls;

		if (tools == null)
		{
			_logger.LogError("Error getting tools");
			return _twilioService.ErrorResponse();
		}
		List<string> functionCalls = new List<string>();
		if (tools.Count > 0)
		{
			functionCalls = await FunctionCaller(tools, user);
		}

		if (message == null && functionCalls.Count == 0)
		{
			_logger.LogError("Error processing GPT response");
			VoiceResponse errorResponse = _twilioService.ErrorResponse();
			return errorResponse;
		}
		else if (message == null && functionCalls.Count > 0)
		{
			functionCalls.Add(
				"Here is a list of the function calls made and the responses from each call. Generate an appropriate response based on the returns from the function calls, if an event is not found return a message to say the event is not found."
			);
			var messageUpdate = string.Join("---", functionCalls);
			_cacheService.UpdateConversationHistory(input.CallSid, messageUpdate);
			var updatedResponse = _cacheService.GetConvoHistory(input.CallSid);
			ChatCompletionData UpdateGPTresponse = await _chatService.GetChatCompletion(
				string.Join("---", updatedResponse ?? new List<string>())
			);
			if (UpdateGPTresponse.Message == null)
			{
				_logger.LogError("Error getting message");
				return _twilioService.ErrorResponse();
			}
			return _twilioService.CallResponse(UpdateGPTresponse.Message);
		}
		if (message == null)
		{
			_logger.LogError("Error getting message");
			return _twilioService.ErrorResponse();
		}
		VoiceResponse? response = _twilioService.CallResponse(message);

		_logger.LogInformation("GPT response successful");
		return response;
	}

	public async Task<VoiceResponse> ProccessCompletedCall(TwilioInputForm input, User user)
	{
		_cacheService.CompleteCall(input.CallSid, input.CallStatus);

		CallHistory? callHistory = _cacheService.GetCallHistory(input.CallSid);
		if (callHistory == null)
		{
			_logger.LogError("Error getting call history");
			return _twilioService.ErrorResponse();
		}

		var request = new AddCallHistoryRequest
		{
			CallHistoryEntity = _mapper.Map<CallHistoryRecord>(callHistory),
		};

		await _callHistoryClient.AddToCallHistoryAsync(request);
		_cacheService.ClearCache(input.CallSid);
		return new VoiceResponse();
	}

	public async Task<List<string>> FunctionCaller(IReadOnlyList<ChatToolCall> tools, User user)
	{
		List<string> messages = new List<string>();

		foreach (ChatToolCall tool in tools)
		{
			string funcName = tool.FunctionName;
			var args = tool.FunctionArguments;

			FunctionCallerArgs? functionCallerArgs = new FunctionCallerArgs
			{
				FunctionName = funcName,
				FunctionArgs = args,
				user = user,
			};

			MethodInfo? methodInfo = _chatToolsFunctions
				.GetType()
				.GetMethod(funcName, BindingFlags.Public | BindingFlags.Instance);

			if (methodInfo == null || functionCallerArgs == null)
			{
				_logger.LogError("Invalid function call");
				messages.Add("Invalid function call: " + funcName);
				continue;
			}

			var invokeResult = methodInfo.Invoke(
				_chatToolsFunctions,
				new object[] { functionCallerArgs }
			);

			if (invokeResult is Task<FunctionCallerReturn> taskResult)
			{
				FunctionCallerReturn result = await taskResult;
				if (!result.Success)
				{
					_logger.LogInformation("Error calling function: " + funcName);
					messages.Add("Error calling function: " + funcName);
				}
				else
				{
					_logger.LogInformation("successful function response here: " + funcName);
					messages.Add("function name: " + funcName + result.Message);
				}
			}
		}
		return messages;
	}
}
