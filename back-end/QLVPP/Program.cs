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
    .AddPresentationServices(configuration)
    .AddCustomMappings()
    .AddGlobalRateLimiting()
    .AddGlobalResponseCompression();

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
app.UseResponseCompression();
app.UseRateLimiter();
app.UseAuthentication();
app.UseMiddleware<RevokedTokenMiddleware>();
app.UseMiddleware<AccountAccessMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
