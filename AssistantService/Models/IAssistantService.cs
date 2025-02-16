using Twilio.TwiML;

namespace AssistantService.Models;

public interface IAssistantService
{
	Task<string> ProcessIncomingRequest(TwilioInputForm input);
	string ProccessInitialCall(TwilioInputForm input, User user);
	Task<string> ProcessInProgressRequests(TwilioInputForm input, User user);
	Task ProccessCompletedCall(TwilioInputForm input, User user);
}
