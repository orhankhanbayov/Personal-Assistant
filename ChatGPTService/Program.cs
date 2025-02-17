using AutoMapper;
using ChatGPTService;
using ChatGPTService.Services;
using ChatGPTService.Utilities;
using Grpc.Net.Client;
using Grpc.Net.ClientFactory;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Chat;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddSingleton<ChatClient>(sp =>
{
	return new ChatClient(model: "gpt-4o", apiKey: builder.Configuration["OpenAI:APIKey"]);
});
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
builder.Services.AddScoped<ChatToolsFunctions>();
var app = builder.Build();
app.UseHttpsRedirection();

app.UseHsts();

// Configure the HTTP request pipeline.
app.MapGrpcService<ChatService>();

app.Run();
