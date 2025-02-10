using Microsoft.Extensions.Caching.Memory;
using PersonalAssistant.Models;

namespace PersonalAssistant.Services;

public class CacheService : ICacheService
{
	private readonly IMemoryCache _cache;

	public CacheService(IMemoryCache cache)
	{
		_cache = cache;
	}

	public List<string> AddOrUpdateConversationHistoryUsingCallHistoryType(
		string callSid,
		string message,
		string callStatus,
		string from,
		string to,
		int UserID
	)
	{
		if (callSid == string.Empty || message == string.Empty || callStatus == string.Empty)
		{
			return new List<string> { };
		}

		if (
			_cache.TryGetValue(callSid, out CallHistory? existingCallTracking)
			&& existingCallTracking != null
		)
		{
			existingCallTracking.CallStatus = callStatus;

			existingCallTracking.ConversationHistory.Add(message);

			_cache.Set(callSid, existingCallTracking, TimeSpan.FromMinutes(5));

			return existingCallTracking.ConversationHistory;
		}
		else
		{
			CallHistory callHistory = new CallHistory
			{
				CallSid = callSid,
				CallStatus = callStatus,
				StartTime = DateTime.Now,
				UserID = UserID,
				To = to,
				From = from,
				ConversationHistory = new List<string> { message },
			};

			_cache.Set(callSid, callHistory, TimeSpan.FromMinutes(5));

			return callHistory.ConversationHistory;
		}
	}

	public CallHistory? GetCallHistory(string callSid)
	{
		if (_cache.TryGetValue(callSid, out CallHistory callHistory))
		{
			return callHistory;
		}
		return null;
	}

	public void ClearCache(string callSid)
	{
		_cache.Remove(callSid);
	}
}
