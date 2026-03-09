using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using QLVPP.Attributes;
using QLVPP.Data;

namespace QLVPP.Filters
{
    public class DynamicAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly AppDbContext _context;

        public DynamicAuthorizeFilter(AppDbContext context)
        {
            _context = context;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.Any(em =>
                em.GetType() == typeof(AllowAnonymousAttribute)
            );

            if (allowAnonymous)
            {
                return;
            }

            var allowCommon = context.ActionDescriptor.EndpointMetadata.Any(em =>
                em.GetType() == typeof(AllowCommonAccessAttribute)
            );

            if (allowCommon)
            {
                var tokenClaim =
                    context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? context
                        .HttpContext.User.FindFirst("id")
                        ?.Value;

                if (string.IsNullOrEmpty(tokenClaim))
                {
                    context.Result = new UnauthorizedResult();
                }
                return;
            }

            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();

            if (string.IsNullOrEmpty(controller) || string.IsNullOrEmpty(action))
            {
                return;
            }

            var idClaim =
                context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? context
                    .HttpContext.User.FindFirst("id")
                    ?.Value;

            if (string.IsNullOrEmpty(idClaim) || !long.TryParse(idClaim, out var userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var permissionName = $"{controller}.{action}";

            var hasPermission = await _context
                .Employees.Where(e => e.Id == userId)
                .SelectMany(e => e.Role.RolePermissions)
                .Select(rp => rp.Permission)
                .AnyAsync(p => p.Name == permissionName);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
