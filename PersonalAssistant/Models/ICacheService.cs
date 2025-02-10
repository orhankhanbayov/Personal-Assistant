using Microsoft.Extensions.Caching.Memory;

namespace PersonalAssistant.Models;

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

	void ClearCache(string callSid);
}

public class CallTracking
{
	public required string CallSid { get; set; }
	public required string CallStatus { get; set; }
	public required List<string> ConversationHistory { get; set; } = new List<string>();
}
