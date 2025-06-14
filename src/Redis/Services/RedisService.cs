using System.Text.Json;
using StackExchange.Redis;
using Redis.Interfaces;

namespace Redis.Services;

public class RedisService(IConnectionMultiplexer connection) : IRedisService
{
    private readonly IDatabase _database = connection.GetDatabase();

    public async Task<T?> GetStringAsync<T>(string key, JsonSerializerOptions? options = null)
    {
        try
        {
            var value = await _database.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return default;

            var result = JsonSerializer.Deserialize<T?>(value!, options);

            return result;
        }
        catch
        {
            return default;
        }
    }

    public async Task<Dictionary<string, T>> GetStringsAsync<T>(string pattern, JsonSerializerOptions? options = null)
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

                result[key!] = JsonSerializer.Deserialize<T?>(value!, options)!;
            }

            return result;
        }
        catch
        {
            return [];
        }
    }

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
