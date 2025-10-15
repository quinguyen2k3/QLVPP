using System.IdentityModel.Tokens.Jwt;
using QLVPP.Services;

namespace QLVPP.Middlewares
{
    public class RevokedTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public RevokedTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IInvalidTokenService invalidTokenService)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                await _next(context);
                return;
            }

            var token = authHeader.Replace("Bearer ", "").Trim();

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jwtToken = null;

            try
            {
                jwtToken = handler.ReadJwtToken(token);
            }
            catch
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token.");
                return;
            }

            var jti = jwtToken
                .Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)
                ?.Value;

            if (string.IsNullOrEmpty(jti))
            {
                await _next(context);
                return;
            }

            var invalidToken = await invalidTokenService.GetById(jti);

            if (invalidToken != null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token has been revoked.");
                return;
            }

            await _next(context);
        }
    }
}
