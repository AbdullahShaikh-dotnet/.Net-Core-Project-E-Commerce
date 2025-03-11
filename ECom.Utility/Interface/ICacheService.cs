using System;
using System.Threading.Tasks;

namespace ECom.Utility.Interface
{
    public interface ICacheService
    {
        /// <summary>
        /// Retrieves a cached string value.
        /// </summary>
        Task<string?> GetStringAsync(string key);

        /// <summary>
        /// Caches a string value with an optional expiration time.
        /// </summary>
        Task<bool> SetStringAsync(string key, string value, TimeSpan? expiry = null);

        /// <summary>
        /// Retrieves a strongly typed object from cache.
        /// </summary>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Caches a strongly typed object with an optional expiration time.
        /// </summary>
        Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiry = null);

        /// <summary>
        /// Removes a cached value.
        /// </summary>
        Task<bool> RemoveAsync(string key);

        /// <summary>
        /// Checks if a key exists in cache.
        /// </summary>
        Task<bool> ExistsAsync(string key);


        /// <summary>
        /// Pub/Sub : Publisher Methods for Real time Publishing Message
        /// </summary>
        Task PublishAsync(string channel, string message);

        /// <summary>
        /// Pub/Sub : Subscriber Methods for Real time Publishing Message
        /// </summary>
        Task SubscribeAsync(string channel, Action<string> handler);

        /// <summary>
        /// Rate Limiting : Prevent API Abuse by limiting per user Request
        /// </summary>
        Task<bool> IsRateLimitedAsync(string key, int limit, TimeSpan duration);
    }
}
