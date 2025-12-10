using System.Text.Json;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace QLVPP.Services.Implementations
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IConnectionMultiplexer multiplexer, ILogger<CacheService> logger)
        {
            _db = multiplexer.GetDatabase();
            _logger = logger;
        }

        public async Task<T> GetOrSet<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
        {
            try
            {
                var cachedValue = await _db.StringGetAsync(key);

                if (!cachedValue.IsNullOrEmpty)
                {
                    var result = JsonSerializer.Deserialize<T>(cachedValue.ToString());
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when write Cache Redis at key: {Key}", key);
            }

            var newValue = await factory();

            if (newValue == null)
            {
                throw new InvalidOperationException(
                    $"Factory returned null for key '{key}'. Non-nullable type required."
                );
            }

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
