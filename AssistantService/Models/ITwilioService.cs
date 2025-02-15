using Twilio.TwiML;

namespace AssistantService.Models;

public interface ITwilioService
{
	VoiceResponse CallResponse(string Message);
	Task<bool> OutgoingCall(string Message, string ToPhoneNumber, string FromPhoneNumber);
	VoiceResponse ErrorResponse();
	bool SendSms(string toPhoneNumber, string message, string FromPhoneNumber);
	Task Notify(string message, string ToPhoneNumber, string FromPhoneNumber);
}
