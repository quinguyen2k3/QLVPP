using QLVPP.Services;
using QLVPP.Services.Implementations;
using StackExchange.Redis;

namespace QLVPP.Extensions
{
    public static class CachingExtensions
    {
        public static IServiceCollection AddCachingServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var redisHost = configuration["Redis:Host"] ?? "localhost";
            var redisPort = configuration["Redis:Port"] ?? "6379";
            var redisPassword = configuration["Redis:Password"] ?? "";

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configOptions = new ConfigurationOptions
                {
                    EndPoints = { $"{redisHost}:{redisPort}" },
                    Password = redisPassword,
                    AbortOnConnectFail = false,
                };
                return ConnectionMultiplexer.Connect(configOptions);
            });

            services.AddSingleton<ICacheService, CacheService>();

            return services;
        }
    }
}
