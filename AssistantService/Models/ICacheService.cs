using Microsoft.Extensions.Caching.Memory;

namespace AssistantService.Models;

public interface ICacheService
{
	List<string> AddOrUpdateConversationHistoryUsingCallHistoryType(
		string key,
		string message,
		string callStatus,
		string from,
		string to,
		int UserID
	);

	CallHistory? GetCallHistory(string callSid);
	List<string>? GetConvoHistory(string callSid);

	CallHistory? CompleteCall(string callSid, string callStatus);

	CallHistory? UpdateConversationHistory(string callSid, string message);

	void ClearCache(string callSid);

	CallHistory? CreateConversationHistory(
		string callSid,
		string callStatus,
		string from,
		string to,
		int UserID,
		string initialMessage,
		User user
	);
}

public class CallTracking
{
	public required string CallSid { get; set; }
	public required string CallStatus { get; set; }
	public required List<string> ConversationHistory { get; set; } = new List<string>();
}
