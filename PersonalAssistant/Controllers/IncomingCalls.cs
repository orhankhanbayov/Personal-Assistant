using DBService;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using PersonalAssistant.Models;
using PersonalAssistant.Services;
using PersonalAssistant.Utilities;
using Twilio.TwiML;

namespace PersonalAssistant.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class IncomingCalls : ControllerBase
	{
		private readonly IAssistantService _assistantService;
		private readonly AppDbContext _dbContext;
		private readonly ILogger<IncomingCalls> _logger;

		public IncomingCalls(
			IAssistantService assistantService,
			AppDbContext dbContext,
			ILogger<IncomingCalls> logger
		)
		{
			_assistantService = assistantService;
			_dbContext = dbContext;
			_logger = logger;
		}

		[HttpPost("InitialCall")]
		public async Task<IActionResult> InitialCall([FromForm] TwilioInputForm input)
		{
			try
			{
				if (!TryValidateModel(input))
				{
					_logger.LogError("Invalid input");
					return BadRequest(ModelState);
				}

				VoiceResponse? response = await _assistantService.ProcessIncomingRequest(input);
				if (response == null)
				{
					_logger.LogError("Response is null");
					return BadRequest("Failed to process the request.");
				}
				return Content(response.ToString(), "text/xml");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "InitialCall failed");
				return BadRequest($"Request failed: {ex.Message}");
			}
		}

		[HttpPost("Callback")]
		public async Task<IActionResult> Callback([FromForm] TwilioInputForm input)
		{
			try
			{
				if (!TryValidateModel(input))
				{
					_logger.LogError("Invalid input");
					return BadRequest(ModelState);
				}

				VoiceResponse? response = await _assistantService.ProcessIncomingRequest(input);
				if (response == null)
				{
					_logger.LogError("Response is null");
					return BadRequest("Failed to process the request.");
				}
				return Content(response.ToString(), "text/xml");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Callback failed");
				return BadRequest($"Request failed: {ex.Message}");
			}
		}

		[HttpPost("TestGRPCWorking")]
		public async Task<IActionResult> TestGRPCWorking()
		{
			try
			{
				var request = new GetUserByPhoneNumberRequest { PhoneNumber = "+447874158451" };
				var channel = GrpcChannel.ForAddress("https://localhost:7098");
				var client = new GetUser.GetUserClient(channel);
				var reply = await client.GetUserByPhoneNumberAsync(request);
				return Ok(reply.UserEntity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "TestGRPCWorking failed");
				return BadRequest($"Request failed: {ex.Message}");
			}
		}
	}
}
