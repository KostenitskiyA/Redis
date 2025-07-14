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

            var data = JsonSerializer.Deserialize<T?>(value!, options);

            return data ?? default;
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
            var result = new Dictionary<string, T>();

            await foreach (var key in server.KeysAsync(pattern: pattern))
            {
                var value = await _database.StringGetAsync(key);

                if (value.IsNullOrEmpty)
                    continue;

                var data = JsonSerializer.Deserialize<T?>(value!, options);

                if (data is null)
                    continue;

                result[key!] = data;
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
