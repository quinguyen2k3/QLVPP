using System.Text.Json;
using StackExchange.Redis;

namespace QLVPP.Services.Implementations
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _db;

        public CacheService(IConnectionMultiplexer multiplexer)
        {
            _db = multiplexer.GetDatabase();
        }

        public async Task<T> GetOrSet<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
        {
            var cachedValue = await _db.StringGetAsync(key);

            if (cachedValue.HasValue)
            {
                return JsonSerializer.Deserialize<T>(cachedValue);
            }

            var newValue = await factory();
            var serializedValue = JsonSerializer.Serialize(newValue);

            await _db.StringSetAsync(key, serializedValue, expiry ?? TimeSpan.FromHours(1));

            return newValue;
        }

        public async Task Remove(string key)
        {
            await _db.KeyDeleteAsync(key);
        }
    }
}
