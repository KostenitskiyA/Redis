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

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var redisConfiguration = ConfigurationOptions.Parse(options.Configuration, true);
            redisConfiguration.AllowAdmin = true;
            return ConnectionMultiplexer.Connect(redisConfiguration);
        });
        services.AddTransient<IRedisService, RedisService, TracingInterceptor>();

        return services;
    }
}
