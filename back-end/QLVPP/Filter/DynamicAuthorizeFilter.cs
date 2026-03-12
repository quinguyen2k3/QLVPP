using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QLVPP.Attributes;
using QLVPP.Services;

namespace QLVPP.Filters
{
    public class DynamicAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly IPermissionService _permissionService;
        private readonly ICurrentUserService _currentUserService;

        public DynamicAuthorizeFilter(
            IPermissionService permissionService,
            ICurrentUserService currentUserService
        )
        {
            _permissionService = permissionService;
            _currentUserService = currentUserService;
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

            if (!_currentUserService.IsUserAuthenticated())
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var allowCommon = context.ActionDescriptor.EndpointMetadata.Any(em =>
                em.GetType() == typeof(AllowCommonAccessAttribute)
            );

            if (allowCommon)
            {
                return;
            }

            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();

            if (string.IsNullOrEmpty(controller) || string.IsNullOrEmpty(action))
            {
                return;
            }

            long userId;
            try
            {
                userId = _currentUserService.GetUserId();
            }
            catch (InvalidOperationException)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var permissionName = $"{controller}.{action}";

            var hasPermission = await _permissionService.HasPermission(userId, permissionName);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
