global using Hangfire;
using SchedulerService;
using SchedulerService.Utilities;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

GlobalConfiguration.Configuration.UseSqlServerStorage(
	builder.Configuration.GetConnectionString("DefaultConnection"));

builder.Services.AddHangfire(configuration =>
	configuration
		.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
		.UseSimpleAssemblyNameTypeSerializer()
		.UseRecommendedSerializerSettings()
);

builder.Services.AddLogging(builder => builder.AddConsole());

builder.Services.AddHangfireServer();
builder.Services.AddAutoMapper(typeof(MapperService));

var grpcDatabaseClientAddress = builder.Configuration["GRPC:DatabaseClient"];
var grpcTwilioClientAddress = builder.Configuration["GRPC:TwilioClient"];


if (string.IsNullOrEmpty(grpcDatabaseClientAddress) || string.IsNullOrEmpty
	(grpcTwilioClientAddress))
{
	var missingConfigs = new List<string>();
	if (string.IsNullOrEmpty(grpcDatabaseClientAddress)) missingConfigs.Add("GRPC:DatabaseClient");
	if (string.IsNullOrEmpty(grpcTwilioClientAddress)) missingConfigs.Add("GRPC:TwilioClient");

	throw new Exception($"Configuration is missing or null for: {string.Join(", ", missingConfigs)}. Exiting application.");
}

builder.Services.AddGrpcClient<Users.UsersClient>(options =>
{
	options.Address = new Uri(grpcDatabaseClientAddress);
});

builder.Services.AddGrpcClient<Notifications.NotificationsClient>(options =>
{
	options.Address = new Uri(grpcDatabaseClientAddress);
});
builder.Services.AddGrpcClient<Tasks.TasksClient>(options =>
{
	options.Address = new Uri(grpcDatabaseClientAddress);
});

builder.Services.AddGrpcClient<Events.EventsClient>(options =>
{
	options.Address = new Uri(grpcDatabaseClientAddress);
});

builder.Services.AddGrpcClient<TwilioPhone.TwilioPhoneClient>(options =>
{
	options.Address = new Uri(grpcTwilioClientAddress);
});
var host = builder.Build();

host.Run();
