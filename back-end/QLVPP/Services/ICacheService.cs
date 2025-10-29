namespace QLVPP.Services
{
    public interface ICacheService
    {
        Task<T> GetOrSet<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null);
        Task Remove(string key);
    }
}
