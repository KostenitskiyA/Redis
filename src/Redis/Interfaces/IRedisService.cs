using System.Text.Json;

namespace Redis.Interfaces;

public interface IRedisService
{
    Task<T?> GetStringAsync<T>(string key);
    
    Task<Dictionary<string, T>> GetStringsDictionaryAsync<T>(string pattern);

    Task<bool> SetStringAsync<T>(
        string key, 
        T data, 
        JsonSerializerOptions? options = null, 
        TimeSpan? expireTime = null);
}