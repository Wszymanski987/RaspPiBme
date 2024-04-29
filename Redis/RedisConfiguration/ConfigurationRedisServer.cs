using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using RaspPiBme.Services;
using RaspPiBme.Redis.ConfigureInitializationServices;
using RaspPiBme.SensorBme280;
using NRedisTimeSeries;

namespace RaspPiBme.Redis.RedisConfiguration
{
    public class ConfigurationRedisServer
    {
        //public IServiceProvider _serviceProvider;
        public ConfigureInitializationTimeSeries? _configureInitializationTimeSeries;
        public IDatabase _database { get; set; }
        public ServicesConfiguration? _servicesConfiguration;
        //private ServiceCollection _serviceDescriptors;
        private ConnectionMultiplexer? _redisMultiplexer;
        private ConfigurationOptions? _redisOptions;

        public void RedisConnectInitialization()
        {
            //var endPointValue = _servicesConfiguration._configuration.GetSection("RedisOption:EndPoints").Value;
            //var passwordValue = _servicesConfiguration._configuration.GetSection("RedisOption:Password").Value;

            var endPointValue = "redis-11448.c311.eu-central-1-1.ec2.cloud.redislabs.com:11448";
            var passwordValue = "FalHPHe0ItdVvgdG7iS9BwXqTa50Dvox";

            if (endPointValue != null && passwordValue != null)
            {
                _redisOptions = new ConfigurationOptions
                {
                    EndPoints = { endPointValue },
                    Password = passwordValue,
                    Ssl = false
                };
                try
                {
                    _redisMultiplexer = ConnectionMultiplexer.Connect(_redisOptions);

                    if (_redisMultiplexer.IsConnected)
                        Console.WriteLine("Connected to Redis server successfully.");

                    _database = _redisMultiplexer.GetDatabase();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error connecting to Redis server: {ex.Message}");
                }
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

        public void showDatabase()
        {
            Console.WriteLine($"Database: {_database}");
        }

        public void setDatabase()
        {
            _database = _redisMultiplexer.GetDatabase();
        }

        public IDatabase GetDatabase()
        {
            return _redisMultiplexer.GetDatabase();
        }
    }
}

