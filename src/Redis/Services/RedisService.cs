using System.Text.Json;
using StackExchange.Redis;
using Redis.Interfaces;

namespace Redis.Services;

public class RedisService(IConnectionMultiplexer connection) : IRedisService
{
    private readonly IDatabase _database = connection.GetDatabase();

    /// <inheritdoc />
    public async Task<T?> GetStringAsync<T>(string key)
    {
        try
        {
            var value = await _database.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return default;

            var result = JsonSerializer.Deserialize<T?>(value!);

            return result;
        }
        catch
        {
            return default;
        }
    }
    
    /// <inheritdoc />
    public async Task<Dictionary<string, T>> GetStringsDictionaryAsync<T>(string pattern)
    {
        try
        {
            var server = connection.GetServer(connection.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern).ToArray();
            
            var result = new Dictionary<string, T>();
            
            foreach (var key in keys)
            {
                var value = await _database.StringGetAsync(key);

                if (value.IsNullOrEmpty)
                    continue;
                
                result[key!] = JsonSerializer.Deserialize<T?>(value!)!;
            }
            
            return result;
        }
        catch
        {
            return [];
        }
    }

    /// <inheritdoc />
    public async Task<bool> SetStringAsync<T>(
        string key,
        T data,
        JsonSerializerOptions? options = null,
        TimeSpan? expireTime = null)
    {
        try
        {
            return await _database.StringSetAsync(
                key,
                JsonSerializer.Serialize(data, options),
                expireTime);
        }
        catch
        {
            return false;
        }
    }
}