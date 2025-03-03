using AssistantService;
using AssistantService.Models;
using AssistantService.Services;
using AssistantService.Utilities;
using OpenTelemetry.Logs;

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

builder.Logging.AddOpenTelemetry(logging => logging.AddOtlpExporter());
builder.Services.AddScoped<IAssistantService, AssistantServiceClass>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MapperService));

// builder.Services.AddAuthentication(); // Add this if you use app.UseAuthentication()
builder.Services.AddAuthorization(); // This is required for app.UseAuthorization()


var grpcDatabaseClientAddress = builder.Configuration["GRPC:DatabaseClient"];
var grpcChatGPTClientAddress = builder.Configuration["GRPC:ChatGPTClient"];
var grpcCacheClientAddress = builder.Configuration["GRPC:CacheClient"];
var grpcTwilioClientAddress = builder.Configuration["GRPC:TwilioClient"];
if (string.IsNullOrEmpty(grpcDatabaseClientAddress) || string.IsNullOrEmpty(grpcChatGPTClientAddress) || string.IsNullOrEmpty(grpcCacheClientAddress) || string.IsNullOrEmpty(grpcTwilioClientAddress))
{
	var missingConfigs = new List<string>();
	if (string.IsNullOrEmpty(grpcDatabaseClientAddress)) missingConfigs.Add("GRPC:DatabaseClient");
	if (string.IsNullOrEmpty(grpcChatGPTClientAddress)) missingConfigs.Add("GRPC:ChatGPTClient");
	if (string.IsNullOrEmpty(grpcCacheClientAddress)) missingConfigs.Add("GRPC:CacheClient");
	if (string.IsNullOrEmpty(grpcTwilioClientAddress)) missingConfigs.Add("GRPC:TwilioClient");

	throw new Exception($"Configuration is missing or null for: {string.Join(", ", missingConfigs)}. Exiting application.");
}


builder.Services.AddGrpcClient<Users.UsersClient>(options =>
{
	options.Address = new Uri(grpcDatabaseClientAddress);
});

builder.Services.AddGrpcClient<CallHistories.CallHistoriesClient>(options =>
{
	options.Address = new Uri(grpcDatabaseClientAddress);
});

builder.Services.AddGrpcClient<Chat.ChatClient>(options =>
{
	options.Address = new Uri(grpcChatGPTClientAddress);
});

builder.Services.AddGrpcClient<Cache.CacheClient>(options =>
{
	options.Address = new Uri(grpcCacheClientAddress);
});

builder.Services.AddGrpcClient<TwilioPhone.TwilioPhoneClient>(options =>
{
	options.Address = new Uri(grpcTwilioClientAddress);
});

var app = builder.Build();
app.UseCors("AllowAll");


app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();


app.UseHsts();
app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowAll");

//app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
