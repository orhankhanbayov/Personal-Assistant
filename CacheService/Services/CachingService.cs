using AutoMapper;
using CacheService;
using CacheService.Models;
using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;

namespace CacheService.Services;

public class CachingService : Cache.CacheBase
{
	private readonly ILogger<CachingService> _logger;
	private readonly IMapper _mapper;
	private readonly IMemoryCache _cache;

	public CachingService(ILogger<CachingService> logger, IMemoryCache cache, IMapper mapper)
	{
		_logger = logger;
		_cache = cache;
		_mapper = mapper;
	}

	public override Task<CreateConversationHistoryReply> CreateConversationHistory(
		CreateConversationHistoryRequest request,
		ServerCallContext context
	)
	{
		CallHistory callHistory = _mapper.Map<CallHistory>(request);

		var result = _cache.Set(request.CallSid, callHistory, TimeSpan.FromMinutes(5));

		return Task.FromResult(new CreateConversationHistoryReply { Success = result != null });
	}

	public override Task<UpdateConversationHistoryReply> UpdateConversationHistory(
		UpdateConversationHistoryRequest request,
		ServerCallContext context
	)
	{
		if (
			_cache.TryGetValue(request.CallSid, out CallHistory? existingCallTracking)
			&& existingCallTracking != null
		)
		{
			existingCallTracking.ConversationHistory.Add(request.Message);
			_cache.Set(request.CallSid, existingCallTracking, TimeSpan.FromMinutes(5));
			return Task.FromResult(new UpdateConversationHistoryReply { Success = true });
		}
		return Task.FromResult(new UpdateConversationHistoryReply { Success = false });
	}

	public override Task<CompleteCallReply> CompleteCall(
		CompleteCallRequest request,
		ServerCallContext context
	)
	{
		if (
			_cache.TryGetValue(request.CallSid, out CallHistory? existingCallTracking)
			&& existingCallTracking != null
		)
		{
			existingCallTracking.CallStatus = request.CallStatus;
			existingCallTracking.EndTime = DateTime.Parse(request.EndTime);
			var returnValue = existingCallTracking;
			_cache.Remove(returnValue.CallSid);
			return Task.FromResult(
				new CompleteCallReply
				{
					CallHistoryEntity = _mapper.Map<CallHistoryType>(existingCallTracking),
				}
			);
		}
		else
		{
			throw new RpcException(new Status(StatusCode.NotFound, "Call history not found"));
		}
	}

	public override Task<GetConvoHistoryReply> GetConvoHistory(
		GetConvoHistoryRequest request,
		ServerCallContext context
	)
	{
		if (
			_cache.TryGetValue(request.CallSid, out CallHistory? callHistory)
			&& callHistory?.ConversationHistory != null
		)
		{
			return Task.FromResult(
				new GetConvoHistoryReply { ConvoHistory = { callHistory.ConversationHistory } }
			);
		}

		throw new RpcException(new Status(StatusCode.NotFound, "Conversation history not found"));
	}

	public void ClearCache(string callSid)
	{
		_cache.Remove(callSid);
	}
}
