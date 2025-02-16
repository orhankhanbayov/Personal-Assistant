using CacheService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<CachingService>();

app.UseHttpsRedirection();

app.UseHsts();

app.Run();
