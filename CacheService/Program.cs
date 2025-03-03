using CacheService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddMemoryCache();

// builder.Services.AddAuthentication(); // Add this if you use app.UseAuthentication()
// builder.Services.AddAuthorization(); // This is required for app.UseAuthorization()

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHsts();
app.UseHttpsRedirection();
app.UseRouting();

// app.UseAuthentication();
// app.UseAuthorization();
app.MapGrpcService<CachingService>();

app.Run();
