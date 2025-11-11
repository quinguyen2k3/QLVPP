namespace QLVPP.Services
{
    public interface ICurrentUserService
    {
        long GetUserId();
        long GetWarehouseId();
        long GetDepartmentId();
        string GetUserAccount();
        bool IsUserAuthenticated();
    }
}
