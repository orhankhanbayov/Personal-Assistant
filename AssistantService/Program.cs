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

// builder.Services.AddAuthentication(); // Add this if you use app.UseAuthentication()
builder.Services.AddAuthorization(); // This is required for app.UseAuthorization()

builder.Services.AddGrpcClient<Users.UsersClient>(options =>
{
	options.Address = new Uri(builder.Configuration["GRPC:DatabaseClient"]);
});

builder.Services.AddGrpcClient<CallHistories.CallHistoriesClient>(options =>
{
	options.Address = new Uri(builder.Configuration["GRPC:DatabaseClient"]);
});

builder.Services.AddGrpcClient<Chat.ChatClient>(options =>
{
	options.Address = new Uri(builder.Configuration["GRPC:ChatGPTClient"]);
});

builder.Services.AddGrpcClient<Cache.CacheClient>(options =>
{
	options.Address = new Uri(builder.Configuration["GRPC:CacheClient"]);
});

builder.Services.AddGrpcClient<TwilioPhone.TwilioPhoneClient>(options =>
{
	options.Address = new Uri(builder.Configuration["GRPC:TwilioClient"]);
});

var app = builder.Build();
app.UseCors("AllowAll");

// if (app.Environment.IsDevelopment())
// {
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

// }

app.UseHsts();
app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowAll");

//app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
