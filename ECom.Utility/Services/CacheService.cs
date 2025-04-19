using ECom.Utility;
using ECom.Utility.Interface;
using ECom.Utility.Services;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ECom.Utility.Services
{
    public class CacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDatabase _redisDB;
        private readonly ILogger<CacheService> _logger;
        private readonly ISubscriber _subscriber;

        public CacheService(IConnectionMultiplexer connectionMultiplexer, ILogger<CacheService> logger)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _redisDB = _connectionMultiplexer.GetDatabase();
            _subscriber = _connectionMultiplexer.GetSubscriber();
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a cached string value.
        /// </summary>
        public async Task<string?> GetStringAsync(string key)
        {
            try
            {
                return await _redisDB.StringGetAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving string from cache for key: {Key}", key);
                return null;
            }
        }

        /// <summary>
        /// Caches a string value.
        /// </summary>
        public async Task<bool> SetStringAsync(string key, string value, TimeSpan? expiry = null)
        {
            try
            {
                return await _redisDB.StringSetAsync(key, value, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting string in cache for key: {Key}", key);
                return false;
            }
        }

        /// <summary>
        /// Retrieves a strongly typed object from cache.
        /// </summary>
        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var value = await _redisDB.StringGetAsync(key);
                return value.HasValue ? JsonSerializer.Deserialize<T>(value!, new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = false
                }) : default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving object from cache for key: {Key}", key);
                return default;
            }
        }

        /// <summary>
        /// Caches a strongly typed object.
        /// </summary>
        public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value,
                    new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve,
                        WriteIndented = false
                    });

                return await _redisDB.StringSetAsync(key, serializedValue, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting object in cache for key: {Key}", key);
                return false;
            }
        }

        /// <summary>
        /// Removes a cached value.
        /// </summary>
        public async Task<bool> RemoveAsync(string key)
        {
            try
            {
                return await _redisDB.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing key from cache: {Key}", key);
                return false;
            }
        }

        /// <summary>
        /// Checks if a key exists in cache.
        /// </summary>
        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return await _redisDB.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if key exists in cache: {Key}", key);
                return false;
            }
        }



        // ======================== PUB/SUB ========================
        public async Task PublishAsync(string channel, string message)
        {
            var pub = _connectionMultiplexer.GetSubscriber();
            await pub.PublishAsync(channel, message);
        }

        public async Task SubscribeAsync(string channel, Action<string> handler)
        {
            var sub = _connectionMultiplexer.GetSubscriber();
            sub.Subscribe(channel, (redisChannel, value) =>
            {
                handler(value.ToString());
            });

            await Task.CompletedTask;
        }

        // ======================== RATE LIMITING ========================
        public async Task<bool> IsRateLimitedAsync(string key, int limit, TimeSpan duration)
        {
            long count = await _redisDB.StringIncrementAsync(key);

            if (count == 1)
                await _redisDB.KeyExpireAsync(key, duration);

            return count > limit;
        }
    }
}


// USAGE

// 1. API Rate Limiting per user (Max Attempt: SD.RATE_LIMITING_COUNT)
//string key = $"rate_limit:{_userService.GetUserId()}";
//if (await _cacheService.IsRateLimitedAsync(key, SD.RATE_LIMITING_COUNT, TimeSpan.FromSeconds(1)))
//    return StatusCode(429, "Too many requests. Try again later.");


//// 2. Check if data is available in Redis cache
//var cachedModel = await _cacheService.GetAsync<ProductFilterViewModel>("ProductObject");
//if (cachedModel != null)
//{
//    return View(cachedModel);
//}


//// 3. Storing Cache
//await _cacheService.SetAsync("ProductObject", query, TimeSpan.FromSeconds(30));