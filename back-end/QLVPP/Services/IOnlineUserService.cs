namespace QLVPP.Services
{
    public interface IOnlineUserService
    {
        Task AddUser(string userId);
        Task RemoveUser(string userId);
        Task<int> GetOnlineUserCount();
    }
}
