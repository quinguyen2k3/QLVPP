using AspNetCoreRateLimit;
using QLVPP.Extensions;
using QLVPP.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// ================== ĐĂNG KÝ CÁC DỊCH VỤ ==================
builder
    .Services.AddDatabaseServices(configuration)
    .AddCachingServices(configuration)
    .AddIdentityServices(configuration)
    .AddApplicationServices()
    .AddPresentationServices(configuration);

builder.Services.AddControllers();

var app = builder.Build();

// ================== SEED DATA ==================
await app.SeedDataAsync();

// ================== CẤU HÌNH MIDDLEWARE PIPELINE ==================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseIpRateLimiting();
app.UseMiddleware<AccountAccessMiddleware>();
app.UseMiddleware<RevokedTokenMiddleware>();
app.MapControllers();

app.Run();
