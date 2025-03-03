using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio.Clients;
using Twilio.Http;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
using Twilio.TwiML.Voice;
using TwilioService;
using TwilioService.Models;
using TwilioService.Services;

namespace TwilioService.Services
{
	public class TwilioPhoneService : TwilioPhone.TwilioPhoneBase
	{
		private readonly ITwilioRestClient _twilioClient;
		private readonly ILogger<TwilioPhoneService> _logger;
		private readonly NgrokOptions _ngrokOptions;

		public TwilioPhoneService(
			ITwilioRestClient twilioClient,
			ILogger<TwilioPhoneService> logger,
			IOptions<NgrokOptions> ngrokOptions
		)
		{
			_twilioClient = twilioClient;
			_logger = logger;
			_ngrokOptions = ngrokOptions.Value;
		}

		public override Task<CallResponseReply> CallResponse(
			CallResponseRequest request,
			ServerCallContext context
		)
		{
			VoiceResponse response = new VoiceResponse();
			Gather responseGather = new Gather(
				input: new[] { Gather.InputEnum.Speech },
				action: new Uri(
					_ngrokOptions.BaseUrl + "/IncomingCalls/InitialCall",
					UriKind.Absolute
				),
				speechTimeout: "auto",
				speechModel: "experimental_conversations"
			);

			responseGather.Say(request.Message);
			response.Append(responseGather);
			response.Say("We did not receive any input. Goodbye!");

			return System.Threading.Tasks.Task.FromResult(
				new CallResponseReply { Response = response.ToString() }
			);
		}

		public override async Task<OutgoingCallReply> OutgoingCall(
			OutgoingCallRequest request,
			ServerCallContext context
		)
		{
			bool success = await MakeOutgoingCall(
				request.Message,
				request.ToPhoneNumber,
				request.FromPhoneNumber
			);
			return new OutgoingCallReply { Success = success };
		}

		private async Task<bool> MakeOutgoingCall(
			string message,
			string toPhoneNumber,
			string fromPhoneNumber
		)
		{
			CallResource call = await CallResource.CreateAsync(
				method: Twilio.Http.HttpMethod.Get,
				twiml: new Twilio.Types.Twiml(message),
				statusCallback: new Uri(_ngrokOptions.BaseUrl + "/IncomingCalls/Callback"),
				statusCallbackMethod: Twilio.Http.HttpMethod.Post,
				to: new Twilio.Types.PhoneNumber(toPhoneNumber),
				from: new Twilio.Types.PhoneNumber(fromPhoneNumber),
				client: _twilioClient
			);

			if (!string.IsNullOrEmpty(call.Sid))
			{
				_logger.LogInformation("Outgoing call made successfully");
				return true;
			}
			else
			{
				_logger.LogError("Error making outgoing call");
				return false;
			}
		}

		public override Task<ErrorResponseReply> ErrorResponse(
			ErrorResponseRequest request,
			ServerCallContext context
		)
		{
			var response = new VoiceResponse();
			response.Say(request.Message);

			return System.Threading.Tasks.Task.FromResult(
				new ErrorResponseReply { Response = response.ToString() }
			);
		}

		public override async Task<NotifyReply> Notify(
			NotifyRequest request,
			ServerCallContext context
		)
		{
			bool success = await MakeOutgoingCall(
				request.Message,
				request.ToPhoneNumber,
				request.FromPhoneNumber
			);
			return new NotifyReply { Success = success };
		}
	}
}
