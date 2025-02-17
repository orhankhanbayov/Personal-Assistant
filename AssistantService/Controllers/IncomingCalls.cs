using AssistantService.Models;
using AssistantService.Services;
using AssistantService.Utilities;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace PersonalAssistant.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class IncomingCalls : ControllerBase
	{
		private readonly IAssistantService _assistantService;
		private readonly ILogger<IncomingCalls> _logger;

		public IncomingCalls(IAssistantService assistantService, ILogger<IncomingCalls> logger)
		{
			_assistantService = assistantService;
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

				string response = await _assistantService.ProcessIncomingRequest(input);
				if (response == null)
				{
					_logger.LogError("Response is null");
					return BadRequest("Failed to process the request.");
				}
				return Content(response, "text/xml");
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

				string response = await _assistantService.ProcessIncomingRequest(input);
				if (response == null)
				{
					_logger.LogError("Response is null");
					return BadRequest("Failed to process the request.");
				}
				return Content(response, "text/xml");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Callback failed");
				return BadRequest($"Request failed: {ex.Message}");
			}
		}
	}
}
