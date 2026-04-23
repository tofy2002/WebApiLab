using Microsoft.Extensions.Caching.Distributed;

namespace Lab3.Services
{
    public class CacheService
    {
        private readonly IDistributedCache _cache;
        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }
        public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(5)
            };
            var jsonData = System.Text.Json.JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, jsonData, options);
        }
        public async Task<T?> GetAsync<T>(string key)
        {
            var jsonData = await _cache.GetStringAsync(key);
            if (jsonData == null)
                return default(T);
            return System.Text.Json.JsonSerializer.Deserialize<T>(jsonData);
        }

        internal async Task RemoveAsync(string v)
        {
            await _cache.RemoveAsync(v);
        }
    }
}
