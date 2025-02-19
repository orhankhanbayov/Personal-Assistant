using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using AssistantService;
using AssistantService.Models;
using AssistantService.Utilities;
using AutoMapper;
using Grpc.Net.Client;

namespace AssistantService.Services;

public class AssistantServiceClass : IAssistantService
{
	private readonly ILogger<AssistantServiceClass> _logger;
	private readonly IMapper _mapper;
	private readonly CallHistories.CallHistoriesClient _callHistoryClient;
	private readonly Users.UsersClient _usersClient;
	private readonly Chat.ChatClient _chatClient;
	private readonly Cache.CacheClient _cacheClient;
	private readonly TwilioPhone.TwilioPhoneClient _twilioClient;

	public AssistantServiceClass(
		ILogger<AssistantServiceClass> logger,
		IMapper mapper,
		CallHistories.CallHistoriesClient callHistoryClient,
		Users.UsersClient usersClient,
		Chat.ChatClient chatClient,
		Cache.CacheClient cacheClient,
		TwilioPhone.TwilioPhoneClient twilioClient
	)
	{
		_logger = logger;
		_mapper = mapper;
		_callHistoryClient = callHistoryClient;
		_usersClient = usersClient;
		_chatClient = chatClient;
		_cacheClient = cacheClient;
		_twilioClient = twilioClient;
	}

	public async Task<string> ProcessIncomingRequest(TwilioInputForm input)
	{
		var request = new GetUserByPhoneNumberRequest { PhoneNumber = input.From };
		var errorResponse = _twilioClient.ErrorResponse(
			new ErrorResponseRequest
			{
				Message = "An unexpected error occurred. Please try again later.",
			}
		);

		var reply = await _usersClient.GetUserByPhoneNumberAsync(request);
		User? user = _mapper.Map<User>(reply.UserEntity);

		if (user == null)
		{
			_logger.LogError("User not found");

			return errorResponse.Response;
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
			await ProccessCompletedCall(input, user);
			return "Call Complete.";
		}
		_logger.LogError("Invalid call status");
		return errorResponse.Response;
	}

	public string ProccessInitialCall(TwilioInputForm input, User user)
	{
		DateTime currentDateTime = DateTime.Now;
		string initialMessage =
			$"You are an intelligent chatbot integrated with a call system. You have functions that you are able to call, you also manage an event calendar and a task list. Your responses are based on real-time transcripts provided by Twilio's machine learning transcription service. Each segment of the conversation will be separated by '---'. You have access to advanced functions to enhance your responses and provide assistance as needed. Ensure clarity, accuracy, and natural interaction when responding to these transcripts. When responding, use the current date {currentDateTime} as a reference point. Always return outputs in the strucutred data format always including a final answer. Current functions available: AddToCalendarAsync to add events to the calendar, ReadFromCalendarTodayAsync to read from the calendar for today, RemoveFromCalendarAsync to remove an event from the calendar, GetEventDetailsAsync To get a specific event, UpdateCalendarEventAsync to update a specific event and CreateTaskAsync is to create a to reminder task seperate from an event.";

		_cacheClient.CreateConversationHistory(
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

		var initialResponse = _twilioClient.CallResponse(
			new CallResponseRequest
			{
				Message = "Welcome I am GPT your assistant, what would you like to do?",
			}
		);

		return initialResponse.Response;
	}

	public async Task<string> ProcessInProgressRequests(TwilioInputForm input, User user)
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
			var errorResponse = _twilioClient.ErrorResponse(
				new ErrorResponseRequest
				{
					Message = "An unexpected error occurred. Please try again later.",
				}
			);
			return errorResponse.Response;
		}

		var gptResponse = await _chatClient.GetChatCompletionAsync(
			new GetChatCompletionRequest { UserMessage = { convoHistory }, UserID = user.UserID }
		);

		var initialResponse = _twilioClient.CallResponse(
			new CallResponseRequest { Message = gptResponse.Message }
		);

		return initialResponse.Response;
	}

	public async Task ProccessCompletedCall(TwilioInputForm input, User user)
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
	}
}
