using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QLVPP.Repositories;
using QLVPP.Services;

namespace QLVPP.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public PermissionService(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<bool> HasPermission(long userId, string permissionName)
        {
            var cacheKey = $"UserPermissions_{userId}";

            var permissions = await _cacheService.GetOrSet(
                cacheKey,
                async () =>
                {
                    var perms = await _unitOfWork.Permission.GetPermissionsByEmployeeId(userId);
                    return new HashSet<string>(perms);
                },
                TimeSpan.FromMinutes(30)
            );

            return permissions.Contains(permissionName);
        }
    }
}
