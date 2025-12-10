namespace QLVPP.Services
{
    public interface IOnlineUserService
    {
        Task AddUser(long userId);
        Task RemoveUser(long userId);
        Task<int> GetOnlineUserCount();
    }
}
