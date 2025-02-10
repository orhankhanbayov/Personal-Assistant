using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;
using PersonalAssistant.Models;
using PersonalAssistant.Utilities;
using Twilio.TwiML;

namespace PersonalAssistant.Services;

public class AssistantService : IAssistantService
{
	private readonly ITwilioService _twilioService;
	private readonly AppDbContext _dbContext;
	private readonly IChatGPTService _chatService;
	private readonly ICacheService _cacheService;

	private readonly ILogger<AssistantService> _logger;

	public AssistantService(
		ITwilioService twilioService,
		AppDbContext appDbContext,
		IChatGPTService chatService,
		ICacheService cacheService,
		ILogger<AssistantService> logger
	)
	{
		_twilioService = twilioService;
		_dbContext = appDbContext;
		_chatService = chatService;
		_cacheService = cacheService;
		_logger = logger;
	}

	public async Task<VoiceResponse> ProcessIncomingRequest(TwilioInputForm input)
	{
		User? user = await _dbContext
			.Users.Where(u => u.PhoneNumber == input.From)
			.FirstOrDefaultAsync();
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
			// fix proper error handling and return type
			return ProccessCompletedCall(input, user);
		}
		_logger.LogError("Invalid call status");
		return errorResponse;
	}

	public VoiceResponse ProccessInitialCall(TwilioInputForm input, User user)
	{
		DateTime currentDateTime = DateTime.Now;
		string initialMessage =
			$"You are an intelligent chatbot integrated with a call system. Your responses are based on real-time transcripts provided by Twilio's machine learning transcription service. Each segment of the conversation will be separated by '---'. You have access to advanced functions to enhance your responses and provide assistance as needed. Ensure clarity, accuracy, and natural interaction when responding to these transcripts. When responding, use the current date {currentDateTime} as a reference point unless the user specifies otherwise. Always return a message especially with tool calls.";

		_cacheService.AddOrUpdateConversationHistoryUsingCallHistoryType(
			input.CallSid,
			initialMessage,
			input.CallStatus,
			input.From,
			input.To,
			user.UserID
		);
		VoiceResponse intialResponse = _twilioService.CallResponse(
			"Welcome I am GPT your assistant, what would you like to do?"
		);
		return intialResponse;
	}

	public async Task<VoiceResponse> ProcessInProgressRequests(TwilioInputForm input, User user)
	{
		List<string> convoHistory =
			_cacheService.AddOrUpdateConversationHistoryUsingCallHistoryType(
				input.CallSid,
				input.SpeechResult ?? "",
				input.CallStatus,
				input.From,
				input.To,
				user.UserID
			);
		if (convoHistory.Count <= 0)
		{
			_logger.LogError("Error updating conversation history");
			return _twilioService.ErrorResponse();
		}

		string messages = string.Join("---", convoHistory);

		ChatCompletionData GPTresponse = await _chatService.GetChatCompletion(messages);

		string? message = GPTresponse.Message;

		IReadOnlyList<ChatToolCall>? tools = GPTresponse.ToolCalls;

		if (tools == null)
		{
			_logger.LogError("Error getting tools");
			return _twilioService.ErrorResponse();
		}

		bool functionCalls = await FunctionCaller(tools, user);

		if (message == null && !functionCalls)
		{
			_logger.LogError("Error processing GPT response");
			VoiceResponse errorResponse = _twilioService.ErrorResponse();
			return errorResponse;
		}
		else if (message == null && functionCalls)
		{
			_logger.LogInformation("Function calls successful");
			return _twilioService.CallResponse("Your request was successful.");
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

	public VoiceResponse ProccessCompletedCall(TwilioInputForm input, User user)
	{
		_cacheService.AddOrUpdateConversationHistoryUsingCallHistoryType(
			input.CallSid,
			input.SpeechResult ?? "",
			input.CallStatus,
			input.From,
			input.To,
			user.UserID
		);
		CallHistory callHistory = _cacheService.GetCallHistory(input.CallSid);
		if (callHistory == null)
		{
			_logger.LogError("Error getting call history");
			return _twilioService.ErrorResponse();
		}
		_dbContext.CallHistories.Add(callHistory);
		_dbContext.SaveChanges();
		_cacheService.ClearCache(input.CallSid);
		return new VoiceResponse();
	}

	public async Task<bool> FunctionCaller(IReadOnlyList<ChatToolCall> tools, User user)
	{
		// create a class for return type - error messages and success messages
		// handle errors for where there's more than 1 tool
		if (tools.Count == 0)
		{
			_logger.LogInformation("No function calls to make");
			return false;
		}

		ChatToolsFunctions chatTools = new ChatToolsFunctions(_dbContext);

		foreach (ChatToolCall tool in tools)
		{
			string funcName = tool.FunctionName;
			var args = tool.FunctionArguments;

			if (string.IsNullOrWhiteSpace(funcName) || args == null)
			{
				_logger.LogError("Invalid function call");
				return false;
			}

			FunctionCallerArgs functionCallerArgs = new FunctionCallerArgs
			{
				FunctionName = funcName,
				FunctionArgs = args,
				user = user,
			};

			MethodInfo? methodInfo = chatTools
				.GetType()
				.GetMethod(funcName, BindingFlags.Public | BindingFlags.Instance);

			if (methodInfo == null)
			{
				_logger.LogError("Invalid function call");
				return false;
			}

			var invokeResult = methodInfo.Invoke(chatTools, new object[] { functionCallerArgs });

			if (invokeResult is Task<bool> taskResult)
			{
				bool result = await taskResult;
				if (!result)
				{
					_logger.LogError("Error calling function");
					return false;
				}
			}
			else
			{
				_logger.LogError("Error calling function");
				return false;
			}
		}
		_logger.LogInformation("Function calls successful");
		return true;
	}
}
