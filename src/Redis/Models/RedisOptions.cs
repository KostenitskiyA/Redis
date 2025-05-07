namespace Redis.Models;

public sealed class RedisConfiguration
{
    public required string Configuration { get; init; }

    public required string InstanceName { get; init; }
}