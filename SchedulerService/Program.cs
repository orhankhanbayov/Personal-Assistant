global using Hangfire;
using System;
using AutoMapper;
using SchedulerService;
using SchedulerService.Utilities;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

// Hangfire services
builder.Services.AddHangfire(configuration =>
	configuration
		.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
		.UseSimpleAssemblyNameTypeSerializer()
		.UseRecommendedSerializerSettings()
		.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddLogging(builder => builder.AddConsole());

builder.Services.AddHangfireServer();
builder.Services.AddAutoMapper(typeof(MapperService));

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

builder.Services.AddGrpcClient<TwilioPhone.TwilioPhoneClient>(options =>
{
	options.Address = new Uri("https://localhost:7139");
});

//app.UseHangfireDashboard();

var host = builder.Build();
host.Run();
