using System.Text.Json;

namespace Redis.Interfaces;

public interface IRedisService
{
    Task<T?> GetStringAsync<T>(string key, JsonSerializerOptions? options = null);

    Task<Dictionary<string, T>> GetStringsAsync<T>(string pattern, JsonSerializerOptions? options = null);

    Task<bool> SetStringAsync<T>(
        string key,
        T data,
        JsonSerializerOptions? options = null,
        TimeSpan? expireTime = null);

    Task<bool> DeleteKeyAsync(string key);

    Task<long> DeleteKeysAsync(string[] keys);
}
