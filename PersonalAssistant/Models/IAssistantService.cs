using OpenAI.Chat;
using Twilio.TwiML;

namespace PersonalAssistant.Models;

public interface IAssistantService
{
	Task<VoiceResponse> ProcessIncomingRequest(TwilioInputForm input);
	VoiceResponse ProccessInitialCall(TwilioInputForm input, User user);
	Task<VoiceResponse> ProcessInProgressRequests(TwilioInputForm input, User user);
	VoiceResponse ProccessCompletedCall(TwilioInputForm input, User user);
	Task<bool> FunctionCaller(IReadOnlyList<ChatToolCall> tools, User user);
}
