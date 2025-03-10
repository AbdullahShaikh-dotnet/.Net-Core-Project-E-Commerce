using ECom.Utility.Interface;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECom.Utility.Services
{
    public class CacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDatabase _redisDB;

        public CacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _redisDB = _connectionMultiplexer.GetDatabase();
        }

        public async Task<string> GetCacheStringAync(string key)
        {
            return await _redisDB.StringGetAsync(key);
        }

        public async Task SetCacheStringAync(string key, string value)
        {
            await _redisDB.StringSetAsync(key, value);
        }

        public async Task<T> GetCacheValueAsync<T>(string key)
        {
            var value = await _redisDB.StringGetAsync(key);
            return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default;
        }

        public async Task SetCacheValueAsync<T>(string key, T value)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            await _redisDB.StringSetAsync(key, serializedValue);
        }
    }
}
