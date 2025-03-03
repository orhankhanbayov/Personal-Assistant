using System.ComponentModel.DataAnnotations;
namespace AssistantService.Models;
public class TwilioInputForm
{
	[Required(ErrorMessage = "CallSid is required.")]
	public required string CallSid { get; set; }

	[Required(ErrorMessage = "fromNumber is required.")]
	public required string From { get; set; }

	[Required(ErrorMessage = "toNumber is required.")]
	public required string To { get; set; }

	[Required(ErrorMessage = "callStatus is required.")]
	public required string CallStatus { get; set; }

	public string? SpeechResult { get; set; }
}
