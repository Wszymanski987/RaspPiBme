using System;
namespace RaspPiBme.Redis.RedisServer.Configuration
{
    public interface IRedisServerConfiguration
    {
        Task RedisConnectInitialization();

        Task RedisDisconnect();
    }
}

