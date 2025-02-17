using Twilio.Clients;
using TwilioService;
using TwilioService.Models;
using TwilioService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddSingleton<ITwilioRestClient>(sp =>
{
	string? accountSid = builder.Configuration["Twilio:AccountSid"];
	string? authToken = builder.Configuration["Twilio:AuthToken"];

	return new TwilioRestClient(accountSid, authToken);
});
builder.Services.Configure<NgrokOptions>(builder.Configuration.GetSection("Ngrok"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<TwilioPhoneService>();

app.UseHttpsRedirection();

app.UseHsts();
app.Run();
