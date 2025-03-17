using Twilio.Clients;
using TwilioService;
using TwilioService.Models;
using TwilioService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

// builder.Services.AddAuthentication(); // Add this if you use app.UseAuthentication()
builder.Services.AddAuthorization();

builder.Services.AddSingleton<ITwilioRestClient>(sp =>
{
	string? accountSid = builder.Configuration["Twilio:AccountSid"];
	string? authToken = builder.Configuration["Twilio:AuthToken"];

	return new TwilioRestClient(accountSid, authToken);
});
builder.Services.Configure<NgrokOptions>(builder.Configuration.GetSection("Ngrok"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHsts();
app.UseHttpsRedirection();

app.UseRouting();

//app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<TwilioPhoneService>();

app.Run();
