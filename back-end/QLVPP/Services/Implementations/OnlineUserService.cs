using StackExchange.Redis;

namespace QLVPP.Services.Implementations
{
    public class OnlineUserService : IOnlineUserService
    {
        private readonly IDatabase _redisDb;
        private const string OnlineUserSetKey = "online_users";

        public OnlineUserService(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        public async Task AddUser(string userId)
        {
            await _redisDb.SetAddAsync(OnlineUserSetKey, userId);
        }

        public async Task<int> GetOnlineUserCount()
        {
            var count = await _redisDb.SetLengthAsync(OnlineUserSetKey);
            return (int)count;
        }

        public async Task RemoveUser(string userId)
        {
            await _redisDb.SetRemoveAsync(OnlineUserSetKey, userId);
        }
    }
}
