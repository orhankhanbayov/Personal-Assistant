using Twilio.TwiML;

namespace AssistantService.Models;

public interface IAssistantService
{
	Task<VoiceResponse> ProcessIncomingRequest(TwilioInputForm input);
	VoiceResponse ProccessInitialCall(TwilioInputForm input, User user);
	Task<VoiceResponse> ProcessInProgressRequests(TwilioInputForm input, User user);
	Task<VoiceResponse> ProccessCompletedCall(TwilioInputForm input, User user);
}
