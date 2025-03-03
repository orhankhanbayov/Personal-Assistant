global using Hangfire;
using SchedulerService;
using SchedulerService.Utilities;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

GlobalConfiguration.Configuration.UseSqlServerStorage(
	"Server=sqlserver,1433;Initial Catalog=PersonalAssistantHangFire;User ID=sa;Password=MyStr0ngP@ss!;TrustServerCertificate=True;Encrypt=True;"
);

builder.Services.AddHangfire(configuration =>
	configuration
		// .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
		.UseSimpleAssemblyNameTypeSerializer()
		.UseRecommendedSerializerSettings()
);

builder.Services.AddLogging(builder => builder.AddConsole());

builder.Services.AddHangfireServer();
builder.Services.AddAutoMapper(typeof(MapperService));

var grpcDatabaseService = builder.Configuration["GRPC:DatabaseService"];
var grpcTwilioService = builder.Configuration["GRPC:TwilioService"];

if (string.IsNullOrEmpty(grpcDatabaseService) || string.IsNullOrEmpty
	(grpcTwilioService))
{
	var missingConfigs = new List<string>();
	if (string.IsNullOrEmpty(grpcDatabaseService)) missingConfigs.Add("GRPC:DatabaseClient");
	if (string.IsNullOrEmpty(grpcTwilioService)) missingConfigs.Add("GRPC:TwilioClient");

	throw new Exception($"Configuration is missing or null for: {string.Join(", ", missingConfigs)}. Exiting application.");
}

builder.Services.AddGrpcClient<Users.UsersClient>(options =>
{
	options.Address = new Uri(grpcDatabaseService);
});

builder.Services.AddGrpcClient<Notifications.NotificationsClient>(options =>
{
	options.Address = new Uri(grpcDatabaseService);
});
builder.Services.AddGrpcClient<Tasks.TasksClient>(options =>
{
	options.Address = new Uri(grpcDatabaseService);
});

builder.Services.AddGrpcClient<Events.EventsClient>(options =>
{
	options.Address = new Uri(grpcDatabaseService);
});

builder.Services.AddGrpcClient<TwilioPhone.TwilioPhoneClient>(options =>
{
	options.Address = new Uri(grpcTwilioService);
});
var host = builder.Build();

host.Run();
