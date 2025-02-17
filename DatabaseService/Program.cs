using DBService.Services;
using DBService.Utilities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();
app.UseHttpsRedirection();

app.UseHsts();

// Configure the HTTP request pipeline.
app.MapGrpcService<GetUserService>();
app.MapGrpcService<CallHistoryService>();
app.MapGrpcService<NotificationService>();
app.MapGrpcService<TaskService>();
app.MapGrpcService<EventService>();
app.Run();
