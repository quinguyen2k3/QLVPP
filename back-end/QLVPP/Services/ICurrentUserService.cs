namespace QLVPP.Services
{
    public interface ICurrentUserService
    {
        long GetUserId();
        long GetWarehouseId();
        string GetUserAccount();
        bool IsUserAuthenticated();
    }
}
