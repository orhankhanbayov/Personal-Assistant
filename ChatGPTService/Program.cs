using ChatGPTService;
using ChatGPTService.Services;
using ChatGPTService.Utilities;
using OpenAI.Chat;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

// builder.Services.AddAuthentication(); // Add this if you use app.UseAuthentication()
// builder.Services.AddAuthorization(); // This is required for app.UseAuthorization()

builder.Services.AddSingleton(sp =>
{
	return new ChatClient(model: "gpt-4o", apiKey: builder.Configuration["OpenAI:APIKey"]);
});
builder.Services.AddAutoMapper(typeof(MapperService));

var grpcDatabaseClientAddress = builder.Configuration["GRPC:DatabaseClient"];
if (string.IsNullOrEmpty(grpcDatabaseClientAddress))
{
	var logger = LoggerFactory.Create(logging => logging.AddConsole()).CreateLogger("Program");
	logger.LogError("GRPC:DatabaseClient configuration is missing or null. Exiting application.");
	Environment.Exit(1);
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

builder.Services.AddGrpcClient<CallHistories.CallHistoriesClient>(options =>
{
	options.Address = new Uri(grpcDatabaseClientAddress);
});
builder.Services.AddScoped<ChatToolsFunctions>();
var app = builder.Build();

app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();

// app.UseAuthentication();
// app.UseAuthorization();
app.MapGrpcService<ChatService>();

app.Run();
