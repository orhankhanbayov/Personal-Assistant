global using Hangfire;
using System;
using AutoMapper;
using Grpc.Net.Client;
using Grpc.Net.ClientFactory;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchedulerService;
using SchedulerService.Utilities;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

GlobalConfiguration.Configuration.UseSqlServerStorage(
	"Server=sqlserver,1433;Initial Catalog=PersonalAssistantHangFire;User ID=sa;Password=MyStr0ngP@ss!;TrustServerCertificate=True;Encrypt=True;"
);

builder.Services.AddHangfire(configuration =>
	configuration
		.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
		.UseSimpleAssemblyNameTypeSerializer()
		.UseRecommendedSerializerSettings()
);

builder.Services.AddLogging(builder => builder.AddConsole());

builder.Services.AddHangfireServer();
builder.Services.AddAutoMapper(typeof(MapperService));

builder.Services.AddGrpcClient<Users.UsersClient>(options =>
{
	// options.Address = new Uri(builder.Configuration["GRPC:DatabaseClient"]);
	options.Address = new Uri("https://DatabaseService");
});

builder.Services.AddGrpcClient<Notifications.NotificationsClient>(options =>
{
	options.Address = new Uri("https://DatabaseService");
});
builder.Services.AddGrpcClient<Tasks.TasksClient>(options =>
{
	options.Address = new Uri("https://DatabaseService");
});

builder.Services.AddGrpcClient<Events.EventsClient>(options =>
{
	options.Address = new Uri("https://DatabaseService");
});

builder.Services.AddGrpcClient<TwilioPhone.TwilioPhoneClient>(options =>
{
	options.Address = new Uri("https://TwilioService");
});
var host = builder.Build();

host.Run();
