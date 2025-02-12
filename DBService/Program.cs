using DBService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();
app.UseHttpsRedirection();

//app.UseHsts();

// Configure the HTTP request pipeline.
app.MapGrpcService<GetUserService>();

app.Run();
