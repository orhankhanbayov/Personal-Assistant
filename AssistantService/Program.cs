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

var app = builder.Build();
app.UseCors("AllowAll");
app.UseRouting();

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
