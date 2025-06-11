
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Services.caching
{
    public class CacheServices : IcacheServices
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheServices> _logger;
        private readonly IConnectionMultiplexer _redis;

        public CacheServices(IDistributedCache cache, ILogger<CacheServices> logger, IConnectionMultiplexer multiplexer)
        {
            _cache = cache;
            _logger = logger;
            _redis = multiplexer;
        }
        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            string ret = "";
            try
            {
                ret = await _cache.GetStringAsync(key);
                _logger.LogWarning($"data ret is {ret}");
            }
            catch (Exception ex)
            {
                ret = "";
            }

            return (ret == null) ? null : JsonSerializer.Deserialize<T>(ret);
        }
        public async Task Remove(string key)
        {
            try
            {
                _logger.LogWarning($"key remove is {key}");
                await _cache.RemoveAsync(key);
            }
            catch (Exception x)
            {
                return;
            }
        }

        public async Task SetAsync<T>(string key, T value) where T : class
        {
            try
            {
                _logger.LogWarning($"set key {key}");
                await _cache.SetStringAsync(key, JsonSerializer.Serialize<T>(value));
            }
            catch (Exception x)
            {
                return;
            }

        }

        public async Task<IEnumerable<T>> GetAllfromCache<T>(string ID) where T : class
        {
            var dtoName = typeof(T).Name;
            var entityName = dtoName.Substring(0, dtoName.Length - 3);
            var key = $"{entityName}-{ID}";

            IEnumerable<T> cachedValues = [];
            try
            {
                cachedValues = await GetAsync<IEnumerable<T>>(key);
                _logger.LogWarning($"getall {JsonSerializer.Serialize<IEnumerable<T>>(cachedValues)}");
            }
            catch
            {
                return null;
            }
            return cachedValues;
        }
        public void removeAllKeys()
        {
            var keys = _redis.GetEndPoints();
            foreach (var key in keys)
            {
                var server = _redis.GetServer(key);
                server.FlushDatabase();
            }
        }
    }
}
