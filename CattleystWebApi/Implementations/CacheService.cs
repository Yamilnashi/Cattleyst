using CattleystWebApi.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CattleystWebApi.Implementations
{
    public class CacheService : ICacheService
    {
        private readonly ILogger<CacheService> _logger;
        private readonly IMemoryCache _cache;

        public CacheService(ILogger<CacheService> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            if (_cache.TryGetValue(key, out T? result))
            {
                _logger.LogInformation("Cache hit for key: {key}", key);
                return result;
            }

            _logger.LogInformation("Cache miss for key: {key}", key);
            result = await factory();
            await SetAsync(key, result, expiration);
            return result;
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions()
            {
                SlidingExpiration = expiration ?? TimeSpan.FromMinutes(5)
            };
            _cache.Set(key, value, options);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
