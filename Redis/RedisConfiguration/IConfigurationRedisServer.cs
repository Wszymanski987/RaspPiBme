using System;
namespace RaspPiBme.Redis.RedisConfiguration
{
    public interface IConfigurationRedisServer
    {
        public void RedisConnectInitialization();

        public void Dispose();

    }
}

