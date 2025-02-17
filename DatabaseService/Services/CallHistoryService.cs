using AutoMapper;
using DBService;
using DBService.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace DBService.Services;

public class CallHistoryService : CallHistories.CallHistoriesBase
{
	private readonly ILogger<CallHistoryService> _logger;
	private readonly AppDbContext _dbContext;
	private readonly IMapper _mapper;

	public CallHistoryService(
		ILogger<CallHistoryService> logger,
		AppDbContext dbContext,
		IMapper mapper
	)
	{
		_logger = logger;
		_dbContext = dbContext;
		_mapper = mapper;
	}

	public override async Task<AddCallHistoryReply> AddToCallHistory(
		AddCallHistoryRequest request,
		ServerCallContext context
	)
	{
		var callHistory = _mapper.Map<CallHistory>(request.CallHistoryEntity);

		await _dbContext.CallHistories.AddAsync(callHistory);
		var result = await _dbContext.SaveChangesAsync();

		return new AddCallHistoryReply { Success = result };
	}
}
