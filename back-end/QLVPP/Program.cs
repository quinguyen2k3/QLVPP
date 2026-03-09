using QLVPP.Extensions;
using QLVPP.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.Configure<FileUploadOptions>(configuration.GetSection("FileUpload"));

// ================== ĐĂNG KÝ CÁC DỊCH VỤ ==================
builder
    .Services.AddDatabaseServices(configuration)
    .AddCachingServices(configuration)
    .AddIdentityServices(configuration)
    .AddApplicationServices()
    .AddPresentationServices(configuration)
    .AddCustomMappings()
    .AddGlobalRateLimiting()
    .AddGlobalResponseCompression()
    .AddCustomCors(configuration);

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

app.UseCors("AllowFrontend");

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseResponseCompression();
app.UseRateLimiter();
app.UseAuthentication();
app.UseMiddleware<RevokedTokenMiddleware>();
app.UseMiddleware<AccountAccessMiddleware>();
app.UseAuthorization();

app.UseConfiguredStaticFiles(configuration, "FileUpload");

app.MapControllers();

app.Run();
