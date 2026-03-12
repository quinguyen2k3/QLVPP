namespace QLVPP.Services
{
    public interface IPermissionService
    {
        Task<bool> HasPermission(long userId, string permissionName);
    }
}
