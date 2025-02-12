using DBService;
using DBService.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace DBService.Services;

public class GetUserService : GetUser.GetUserBase
{
	private readonly ILogger<GetUserService> _logger;
	private readonly AppDbContext _dbContext;

	public GetUserService(ILogger<GetUserService> logger, AppDbContext dbContext)
	{
		_logger = logger;
		_dbContext = dbContext;
	}

	public override async Task<GetUserByPhoneNumberReply> GetUserByPhoneNumber(
		GetUserByPhoneNumberRequest request,
		ServerCallContext context
	)
	{
		User? userByPhone = await _dbContext
			.Users.Where(u => u.PhoneNumber == request.PhoneNumber)
			.FirstOrDefaultAsync();

		if (userByPhone == null)
			throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
		var u = new UserMessage
		{
			UserID = userByPhone.UserID,
			FirstName = userByPhone.FirstName,
			LastName = userByPhone.LastName,
			PhoneNumber = userByPhone.PhoneNumber,
			Email = userByPhone.Email,
			CreatedAt = userByPhone.CreatedAt.ToUniversalTime().ToString("o"),
			LastLoginAt = userByPhone.LastLoginAt.HasValue
				? userByPhone.LastLoginAt.Value.ToUniversalTime().ToString("o")
				: null,
		};
		return new GetUserByPhoneNumberReply { UserEntity = u };
	}
}
