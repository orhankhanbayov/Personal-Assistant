using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using AssistantService.Models;
using AssistantService.Utilities;
using AutoMapper;
using Grpc.Net.Client;
using Twilio.TwiML;

namespace AssistantService.Services;

public class AssistantServiceClass : IAssistantService
{
	private readonly ITwilioService _twilioService;
	private readonly ILogger<AssistantServiceClass> _logger;
	private readonly IMapper _mapper;
	private readonly CallHistories.CallHistoriesClient _callHistoryClient;
	private readonly Users.UsersClient _usersClient;
	private readonly Chat.ChatClient _chatClient;
	private readonly Cache.CacheClient _cacheClient;

	public AssistantServiceClass(
		ITwilioService twilioService,
		ILogger<AssistantServiceClass> logger,
		IMapper mapper,
		CallHistories.CallHistoriesClient callHistoryClient,
		Users.UsersClient usersClient,
		Chat.ChatClient chatClient,
		Cache.CacheClient cacheClient
	)
	{
		_twilioService = twilioService;
		_logger = logger;
		_mapper = mapper;
		_callHistoryClient = callHistoryClient;
		_usersClient = usersClient;
		_chatClient = chatClient;
		_cacheClient = cacheClient;
	}

	public async Task<VoiceResponse> ProcessIncomingRequest(TwilioInputForm input)
	{
		var request = new GetUserByPhoneNumberRequest { PhoneNumber = input.From };

		var reply = await _usersClient.GetUserByPhoneNumberAsync(request);
		User? user = _mapper.Map<User>(reply.UserEntity);

		if (user == null)
		{
			_logger.LogError("User not found");
			return _twilioService.ErrorResponse();
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
		return _twilioService.ErrorResponse();
	}

	public VoiceResponse ProccessInitialCall(TwilioInputForm input, User user)
	{
		DateTime currentDateTime = DateTime.Now;
		string initialMessage =
			$"You are an intelligent chatbot integrated with a call system. Your responses are based on real-time transcripts provided by Twilio's machine learning transcription service. Each segment of the conversation will be separated by '---'. You have access to advanced functions to enhance your responses and provide assistance as needed. Ensure clarity, accuracy, and natural interaction when responding to these transcripts. When responding, use the current date {currentDateTime} as a reference point. Always return a message especially with tool calls.";

		var a = _cacheClient.CreateConversationHistory(
			new CreateConversationHistoryRequest
			{
				CallSid = input.CallSid,
				CallStatus = input.CallStatus,
				From = input.From,
				To = input.To,
				UserID = user.UserID,
				InitialMessage = initialMessage,
				User = _mapper.Map<UserRecord>(user),
				StartTime = currentDateTime.ToString("o"),
			}
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

		_cacheClient.UpdateConversationHistory(
			new UpdateConversationHistoryRequest
			{
				CallSid = input.CallSid,
				Message = input.SpeechResult,
			}
		);

		var getConvoHistory = _cacheClient.GetConvoHistory(
			new GetConvoHistoryRequest { CallSid = input.CallSid }
		);

		List<string?>? convoHistory = getConvoHistory.ConvoHistory?.ToList();

		if (convoHistory == null)
		{
			_logger.LogError("Error getting conversation history");
			return _twilioService.ErrorResponse();
		}

		var gptResponse = await _chatClient.GetChatCompletionAsync(
			new GetChatCompletionRequest { UserMessage = { convoHistory }, UserID = user.UserID }
		);
		VoiceResponse? response = _twilioService.CallResponse(gptResponse.Message);
		return response;
	}

	public async Task<VoiceResponse> ProccessCompletedCall(TwilioInputForm input, User user)
	{
		var completeCall = _cacheClient.CompleteCall(
			new CompleteCallRequest
			{
				CallSid = input.CallSid,
				CallStatus = input.CallStatus,
				EndTime = DateTime.Now.ToString("o"),
			}
		);

		CallHistory? callHistory = _mapper.Map<CallHistory>(completeCall.CallHistoryEntity);

		var request = new AddCallHistoryRequest
		{
			CallHistoryEntity = _mapper.Map<CallHistoryRecord>(callHistory),
		};

		await _callHistoryClient.AddToCallHistoryAsync(request);

		return new VoiceResponse();
	}
}
