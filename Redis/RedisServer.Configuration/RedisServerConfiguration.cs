using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using RaspPiBme.Services;
using RaspPiBme.SensorBme280;
using NRedisTimeSeries;
using Microsoft.Extensions.Configuration;

namespace RaspPiBme.Redis.RedisServer.Configuration
{
    public class RedisServerConfiguration
    {
        public IDatabase Database { get; set; }

        private ConnectionMultiplexer? _redisMultiplexer { get; set; }

        private ConfigurationOptions? _redisOptions { get; set; }

        private IConfiguration _configuration { get; set; }

        private string _endPoints { get; set; }

        private string _password { get; set; }

        private bool _ssl { get; set; }

        public RedisServerConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
            _endPoints = _configuration.GetSection("RedisOption:EndPoints").Value;
            _password = _configuration.GetSection("RedisOption:Password").Value;
            _ssl = false;
        }

        public async Task RedisConnectInitialization()
        {
                var redisOptions = new ConfigurationOptions
                {
                    EndPoints = { "redis-11448.c311.eu-central-1-1.ec2.cloud.redislabs.com:11448" },
                    Password = "FalHPHe0ItdVvgdG7iS9BwXqTa50Dvox",
                    Ssl = false
                };
                try
                {
                    _redisMultiplexer = ConnectionMultiplexer.Connect(redisOptions);

                    if (_redisMultiplexer.IsConnected)
                    {
                        Console.WriteLine("Connected to Redis server successfully.");

                    }
                        Database = _redisMultiplexer.GetDatabase();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error connecting to Redis server: {ex.Message}");
                }
            }

        public async Task DisconnectFromRedis()
        {
            if (_redisMultiplexer != null)
            {
                await EnsureRedisIsConnected();
                _redisMultiplexer.Dispose();
            }
        }

        private async Task EnsureRedisIsConnected()
        {
            if (Database == null)
            {
                await RedisConnectInitialization();
            }
        }
    }
}    



