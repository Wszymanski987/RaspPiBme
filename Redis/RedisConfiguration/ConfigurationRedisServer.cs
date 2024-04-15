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


        public ConfigurationRedisServer(ServicesConfiguration servicesConfiguration)
        {
            _servicesConfiguration = servicesConfiguration;
            _serviceDescriptors = new ServiceCollection();
            _serviceProvider = _serviceDescriptors.BuildServiceProvider();
        }

        public void RedisConnectInitialization()
        {
            var redisOptions = new ConfigurationOptions
            {
                EndPoints = { _servicesConfiguration._configuration.GetSection("RedisOption:EndPoints").Value },
                Password = _servicesConfiguration._configuration.GetSection("RedisOption:Password").Value,
                Ssl = false
            };
            try
            {
                _redisMultiplexer = ConnectionMultiplexer.Connect(redisOptions);
                Console.WriteLine("Connected to Redis server successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to Redis server: {ex.Message}");
            }

        }

        public void Dispose()
        {
            _redisMultiplexer?.Dispose();
        }

    }
}

