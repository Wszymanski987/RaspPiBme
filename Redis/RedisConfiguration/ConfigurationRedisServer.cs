using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Text;
using RaspPiBme.Services;

namespace RaspPiBme.Redis.RedisConfiguration
{
    public class ConfigurationRedisServer
    {
        private ServicesConfiguration _servicesConfiguration;
        private ServiceCollection _serviceDescriptors;
        private IServiceProvider _serviceProvider;
        private ConnectionMultiplexer? _redisMultiplexer;

        private ConfigurationOptions? _redisOptions;


        public ConfigurationRedisServer(ServicesConfiguration servicesConfiguration)
        {
            _servicesConfiguration = servicesConfiguration;
            _serviceDescriptors = new ServiceCollection();
            _serviceProvider = _serviceDescriptors.BuildServiceProvider();
        }

        public void RedisConnectInitialization()
        {
            var endPointValue = _servicesConfiguration._configuration.GetSection("RedisOption:EndPoints").Value;
            var passwordValue = _servicesConfiguration._configuration.GetSection("RedisOption:Password").Value;

            if (endPointValue != null && passwordValue != null)
            {
                _redisOptions = new ConfigurationOptions
                {
                    EndPoints = { endPointValue },
                    Password = passwordValue,
                    Ssl = false
                };
                _redisMultiplexer = ConnectionMultiplexer.Connect(_redisOptions);
                Console.WriteLine("Connected to Redis server successfully.");
            }
            else
            {
                throw new Exception("Error: RedisOption:EndPoints or RedisOption:Password is null");
            }

        }

        public void Dispose()
        {
            _redisMultiplexer?.Dispose();
        }

    }
}

