using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class CorsExtensions
{
    public static IServiceCollection AddCustomCors(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        var origins = config.GetSection("Cors:AllowedOrigins").Get<string[]>();

        services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowFrontend",
                builder =>
                {
                    builder
                        .WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
            );
        });

        return services;
    }
}
