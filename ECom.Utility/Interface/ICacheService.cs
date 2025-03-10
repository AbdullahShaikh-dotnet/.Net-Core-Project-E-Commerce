using ECom.Models.InvoiceModels;

namespace ECom.Utility.Interface
{
    public interface ICacheService
    {
        Task<string> GetCacheStringAync(string key);

        Task SetCacheStringAync(string key, string value);

        Task<T> GetCacheValueAsync<T>(string key);

        Task SetCacheValueAsync<T>(string key, T value);
    }
}
