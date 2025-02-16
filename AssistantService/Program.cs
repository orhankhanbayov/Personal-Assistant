global using Hangfire;
using System;
using AssistantService;
using AssistantService.Models;
using AssistantService.Services;
using AssistantService.Utilities;
using AutoMapper;
using Grpc.Net.Client;
using Grpc.Net.ClientFactory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;
using Twilio.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
	options.AddPolicy(
		"AllowAll",
		policy =>
		{
			policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
		}
	);
});

// Twilio
builder.Services.AddSingleton<ITwilioRestClient>(sp =>
{
	string? accountSid = builder.Configuration["Twilio:AccountSid"];
	string? authToken = builder.Configuration["Twilio:AuthToken"];

	return new TwilioRestClient(accountSid, authToken);
});

// --------------------------------------------------------------------

// Hangfire services
builder.Services.AddHangfire(configuration =>
	configuration
		.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
		.UseSimpleAssemblyNameTypeSerializer()
		.UseRecommendedSerializerSettings()
		.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// --------------------------------------------------------------------
builder.Services.AddLogging(builder => builder.AddConsole());

builder.Services.AddHangfireServer();
builder.Logging.AddOpenTelemetry(logging => logging.AddOtlpExporter());
builder.Services.AddScoped<ITwilioService, TwilioService>();
builder.Services.AddScoped<IAssistantService, AssistantServiceClass>();
builder.Services.AddScoped<IBackgroundService, NotifyBackgroundService>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MapperService));
builder.Services.Configure<NgrokOptions>(builder.Configuration.GetSection("Ngrok"));

builder.Services.AddGrpcClient<Users.UsersClient>(options =>
{
	options.Address = new Uri("https://localhost:7098");
});

builder.Services.AddGrpcClient<Notifications.NotificationsClient>(options =>
{
	options.Address = new Uri("https://localhost:7098");
});
builder.Services.AddGrpcClient<Tasks.TasksClient>(options =>
{
	options.Address = new Uri("https://localhost:7098");
});

builder.Services.AddGrpcClient<Events.EventsClient>(options =>
{
	options.Address = new Uri("https://localhost:7098");
});

builder.Services.AddGrpcClient<CallHistories.CallHistoriesClient>(options =>
{
	options.Address = new Uri("https://localhost:7098");
});

builder.Services.AddGrpcClient<Chat.ChatClient>(options =>
{
	options.Address = new Uri("https://localhost:7116");
});
builder.Services.AddGrpcClient<Cache.CacheClient>(options =>
{
	options.Address = new Uri("https://localhost:7278");
});

var app = builder.Build();
app.UseCors("AllowAll");
app.UseRouting();
app.UseHangfireDashboard();

using (var scope = app.Services.CreateScope())
{
	var serviceProvider = scope.ServiceProvider;
	var recurringJobs = serviceProvider.GetRequiredService<IRecurringJobManager>();
	var backgroundService = serviceProvider.GetRequiredService<IBackgroundService>();

	backgroundService.processOutgoingRequests(recurringJobs);
}

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseHsts();

app.UseAuthorization();

app.MapControllers();

app.Run();
