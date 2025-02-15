using AssistantService.Models;
using Microsoft.Extensions.Caching.Memory;

namespace AssistantService.Services;

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

	public CallHistory? CreateConversationHistory(
		string callSid,
		string callStatus,
		string from,
		string to,
		int UserID,
		string initialMessage,
		User user
	)
	{
		CallHistory callHistory = new CallHistory
		{
			CallSid = callSid,
			CallStatus = callStatus,
			StartTime = DateTime.Now,
			UserID = UserID,
			To = to,
			From = from,
			ConversationHistory = new List<string> { initialMessage },
		};

		_cache.Set(callSid, callHistory, TimeSpan.FromMinutes(5));

		return callHistory;
	}

	public CallHistory? UpdateConversationHistory(string callSid, string message)
	{
		if (
			_cache.TryGetValue(callSid, out CallHistory? existingCallTracking)
			&& existingCallTracking != null
		)
		{
			existingCallTracking.ConversationHistory.Add(message);
			_cache.Set(callSid, existingCallTracking, TimeSpan.FromMinutes(5));
			return existingCallTracking;
		}
		return null;
	}

	public CallHistory? GetCallHistory(string callSid)
	{
		if (_cache.TryGetValue(callSid, out CallHistory? callHistory))
		{
			return callHistory;
		}
		return null;
	}

	public CallHistory? CompleteCall(string callSid, string callStatus)
	{
		if (
			_cache.TryGetValue(callSid, out CallHistory? existingCallTracking)
			&& existingCallTracking != null
		)
		{
			existingCallTracking.CallStatus = callStatus;
			existingCallTracking.EndTime = DateTime.Now;
			_cache.Set(callSid, existingCallTracking, TimeSpan.FromMinutes(5));
			return existingCallTracking;
		}
		return null;
	}

	public List<string>? GetConvoHistory(string callSid)
	{
		if (
			_cache.TryGetValue(callSid, out CallHistory? callHistory)
			&& callHistory?.ConversationHistory != null
		)
		{
			return callHistory.ConversationHistory;
		}

		return null;
	}

	public void ClearCache(string callSid)
	{
		_cache.Remove(callSid);
	}
}
