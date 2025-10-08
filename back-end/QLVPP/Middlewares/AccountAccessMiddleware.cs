using QLVPP.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace QLVPP.Middlewares
{
    public class AccountAccessMiddleware
    {
        private readonly RequestDelegate _next;

        public AccountAccessMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var _currentUserService = context.RequestServices.GetRequiredService<ICurrentUserService>();
            var _employeeService = context.RequestServices.GetRequiredService<IEmployeeService>();

            if (_currentUserService.IsAuthenticated == false)
            {
                await _next(context);
                return;
            }

            var userId = _currentUserService.UserId;
            var account = _currentUserService.UserAccount;

            if (userId == null || string.IsNullOrWhiteSpace(account))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Missing account information.");
                return;
            }

            var user = await _employeeService.GetById(userId.Value);

            if (user == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("User not found.");
                return;
            }

            if (!string.Equals(user.Account, account, StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Account mismatch.");
                return;
            }

            if (user.IsActivated == false) 
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Your account has been disabled.");
                return;
            }

            await _next(context);
        }
    }
}