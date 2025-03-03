using AutoMapper;
using DBService;
using DBService.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace DBService.Services;

public class GetUserService : Users.UsersBase
{
	private readonly ILogger<GetUserService> _logger;
	private readonly AppDbContext _dbContext;
	private readonly IMapper _mapper;

	public GetUserService(ILogger<GetUserService> logger, AppDbContext dbContext, IMapper mapper)
	{
		_logger = logger;
		_dbContext = dbContext;
		_mapper = mapper;
	}

	public override async Task<GetUserByPhoneNumberReply> GetUserByPhoneNumber(
		GetUserByPhoneNumberRequest request,
		ServerCallContext context
	)
	{
		// User? userByPhone = await _dbContext
		// 	.Users.Where(u => u.PhoneNumber == request.PhoneNumber)
		// 	.FirstOrDefaultAsync();
		User? userByPhone = null;
		if (userByPhone == null)
			throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

		userByPhone.LastLoginAt = DateTime.Now;
		await _dbContext.SaveChangesAsync();

		var userMessage = _mapper.Map<UserMessage>(userByPhone);
		return new GetUserByPhoneNumberReply { UserEntity = userMessage };
	}

	public override async Task<GetUserByUserIDReply> GetUserByUUID(
		GetUserByUserIDRequest request,
		ServerCallContext context
	)
	{
		User? userByID = await _dbContext
			.Users.Where(u => u.UUID == request.UUID)
			.FirstOrDefaultAsync();

		if (userByID == null)
			throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

		var userMessage = _mapper.Map<UserMessage>(userByID);
		return new GetUserByUserIDReply { UserEntity = userMessage };
	}

	public override async Task<CreateUserReply> CreateUser(
		CreateUserRequest request,
		ServerCallContext context
	)
	{
		User user = new()
		{
			UUID = request.UUID
		};
		await _dbContext.Users.AddAsync(user);
		await _dbContext.SaveChangesAsync();

		return new CreateUserReply { Success = 1 };
	}
}
