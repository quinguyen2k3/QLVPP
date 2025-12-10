using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace QLVPP.Extensions
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddGlobalRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                    double? retryAfterSeconds = null;

                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        retryAfterSeconds = retryAfter.TotalSeconds;
                    }

                    await context.HttpContext.Response.WriteAsJsonAsync(
                        new
                        {
                            Succeeded = false,
                            Message = "Too many requests. Please try again later.",
                            RetryAfter = retryAfterSeconds,
                        }
                    );
                };

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                    httpContext =>
                    {
                        var ipAddress =
                            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: ipAddress,
                            factory: partition => new FixedWindowRateLimiterOptions
                            {
                                AutoReplenishment = true,
                                PermitLimit = 100,
                                Window = TimeSpan.FromMinutes(1),
                                QueueLimit = 0,
                            }
                        );
                    }
                );

                options.AddPolicy(
                    "StrictPolicy",
                    httpContext =>
                    {
                        var ipAddress =
                            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: ipAddress,
                            factory: partition => new FixedWindowRateLimiterOptions
                            {
                                AutoReplenishment = true,
                                PermitLimit = 10,
                                Window = TimeSpan.FromMinutes(1),
                                QueueLimit = 0,
                            }
                        );
                    }
                );
            });

            return services;
        }
    }
}
