using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Redis.Interfaces;
using Redis.Models;
using Redis.Services;
using Share.Extensions;
using Share.Interceptors;

namespace Redis.Extensions;

public static class RedisExtensions
{
    public static IServiceCollection AddRedis(
        this IServiceCollection services,
        IConfiguration configuration,
        IHealthChecksBuilder? healthChecksBuilder = null)
    {
        var options = services.ConfigureOptions<RedisConfiguration>(configuration);

        healthChecksBuilder?.AddRedis(options.Configuration);

        services.AddTransient<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(options.Configuration));
        services.AddTransient<IRedisService, RedisService, TracingInterceptor>();

        return services;
    }
}
