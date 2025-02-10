global using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using OpenTelemetry.Logs;
using PersonalAssistant.Models;
using PersonalAssistant.Services;
using PersonalAssistant.Utilities;
using Twilio.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
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

// OpenAI
builder.Services.AddSingleton<ChatClient>(sp =>
{
	return new ChatClient(model: "gpt-4o", apiKey: builder.Configuration["OpenAI:APIKey"]);
});

// --------------------------------------------------------------------

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
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<ITwilioService, TwilioService>();
builder.Services.AddScoped<IChatGPTService, ChatGPTService>();
builder.Services.AddScoped<IAssistantService, AssistantService>();
builder.Services.AddScoped<IBackgroundService, NotifyBackgroundService>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
