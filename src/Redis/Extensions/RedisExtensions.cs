using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Redis.Interfaces;
using Redis.Models;
using Redis.Services;

namespace Redis.Extensions;

public static class RedisExtensions
{
    public static IServiceCollection AddRedis(
        this IServiceCollection services,
        IConfiguration configuration,
        IHealthChecksBuilder? healthChecksBuilder = null)
    {
        var section = configuration.GetSection(nameof(RedisConfiguration));
        if (section is null)
            throw new Exception($"{nameof(RedisConfiguration)} section not found");

        var options = section.Get<RedisConfiguration>();
        if (options is null)
            throw new Exception($"{nameof(RedisConfiguration)} options not found");

        healthChecksBuilder?.AddRedis(options.Configuration);

        services.Configure<RedisConfiguration>(section);
        services.AddTransient<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(options.Configuration));
        services.AddTransient<IRedisService, RedisService>();

        return services;
    }
}
