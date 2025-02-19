using DBService.Services;
using DBService.Utilities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// builder.Services.AddAuthentication(); // Add this if you use app.UseAuthentication()
// builder.Services.AddAuthorization(); // This is required for app.UseAuthorization()

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();

// app.UseAuthentication();
// app.UseAuthorization();
// add debugging if connected to db print a string
app.MapGrpcService<GetUserService>();
app.MapGrpcService<CallHistoryService>();
app.MapGrpcService<NotificationService>();
app.MapGrpcService<TaskService>();
app.MapGrpcService<EventService>();
app.Run();
