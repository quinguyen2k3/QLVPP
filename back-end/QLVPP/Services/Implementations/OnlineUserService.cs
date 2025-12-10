using StackExchange.Redis;

namespace QLVPP.Services.Implementations
{
    public class OnlineUserService : IOnlineUserService
    {
        private readonly IDatabase _redisDb;
        private const string OnlineUserSetKey = "online_users";
        private const int TIMEOUT_SECONDS = 30;

        public OnlineUserService(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        public async Task AddUser(long userId)
        {
            double score = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            await _redisDb.SortedSetAddAsync(
                OnlineUserSetKey,
                userId,
                score,
                flags: CommandFlags.FireAndForget
            );
        }

        public async Task<int> GetOnlineUserCount()
        {
            double minScore = double.NegativeInfinity;
            double maxScore = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - TIMEOUT_SECONDS;

            await _redisDb.SortedSetRemoveRangeByScoreAsync(OnlineUserSetKey, minScore, maxScore);

            long count = await _redisDb.SortedSetLengthAsync(OnlineUserSetKey);

            return (int)count;
        }

        public async Task RemoveUser(long userId)
        {
            await _redisDb.SetRemoveAsync(OnlineUserSetKey, userId);
        }
    }
}
