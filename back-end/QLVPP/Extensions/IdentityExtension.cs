using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace QLVPP.Extensions
{
    public static class IdentityExtension
    {
        public static IServiceCollection AddIdentityServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var jwtKey =
                configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key is missing");
            var jwtIssuer =
                configuration["Jwt:Issuer"]
                ?? throw new ArgumentNullException("Jwt:Issuer is missing");
            var jwtAudience =
                configuration["Jwt:Audience"]
                ?? throw new ArgumentNullException("Jwt:Audience is missing");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = key,
                        ClockSkew = TimeSpan.Zero,
                    };
                });

            services.AddAuthorization();

            return services;
        }
    }
}
