using PersonalAssistant.Models;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
using Twilio.TwiML.Voice;

namespace PersonalAssistant.Services;

public class TwilioService : ITwilioService
{
	private readonly ITwilioRestClient _twilioClient;
	private readonly ILogger<ITwilioService> _logger;

	public TwilioService(ITwilioRestClient twilioClient, ILogger<ITwilioService> logger)
	{
		_twilioClient = twilioClient;
		_logger = logger;
	}

	public VoiceResponse CallResponse(string Message)
	{
		VoiceResponse response = new VoiceResponse();
		Gather responseGather = new Gather(
			input: new[] { Gather.InputEnum.Speech },
			action: new Uri(
				"https://630e-82-16-161-103.ngrok-free.app/IncomingCalls/Callback",
				UriKind.Absolute
			),
			speechTimeout: "auto",
			speechModel: "experimental_conversations"
		);

		responseGather.Say(Message);
		response.Append(responseGather);
		response.Say("We did not receive any input. Goodbye!");
		return response;
	}

	// test inputs null return false
	public async Task<bool> OutgoingCall(
		string Message,
		string ToPhoneNumber,
		string FromPhoneNumber
	)
	{
		CallResource call = await CallResource.CreateAsync(
			method: Twilio.Http.HttpMethod.Get,
			twiml: new Twilio.Types.Twiml(Message),
			statusCallback: new Uri(
				"https://630e-82-16-161-103.ngrok-free.app/IncomingCalls/Callback"
			),
			statusCallbackMethod: Twilio.Http.HttpMethod.Post,
			to: new Twilio.Types.PhoneNumber(ToPhoneNumber),
			from: new Twilio.Types.PhoneNumber(FromPhoneNumber),
			client: _twilioClient
		);
		if (!string.IsNullOrEmpty(call.Sid))
		{
			_logger.LogInformation("Outgoing call made successfully");
			return true;
		}
		else
		{
			_logger.LogError("Error making outgoing call");
			return false;
		}
	}

	public VoiceResponse ErrorResponse()
	{
		VoiceResponse response = new VoiceResponse();

		response.Say("An unexpected error occurred. Please try again later.");
		return response;
	}

	public bool SendSms(string toPhoneNumber, string message, string FromPhoneNumber)
	{
		var messageOptions = new CreateMessageOptions(toPhoneNumber)
		{
			From = FromPhoneNumber,
			Body = message,
		};

		var msg = MessageResource.Create(messageOptions);
		if (!string.IsNullOrEmpty(msg.Sid))
		{
			_logger.LogInformation("SMS sent successfully");
			return true;
		}
		else
		{
			_logger.LogError("Error sending SMS");
			return false;
		}
	}

	public async System.Threading.Tasks.Task Notify(
		string message,
		string ToPhoneNumber,
		string FromPhoneNumber
	)
	{
		var call = await OutgoingCall(message, ToPhoneNumber, FromPhoneNumber);
		if (!call)
		{
			_logger.LogError("Error making outgoing call");
		}
	}
}
