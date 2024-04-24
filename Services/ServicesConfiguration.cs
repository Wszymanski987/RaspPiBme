using System;
using RaspPiBme.Redis.ConfigureInitializationServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RaspPiBme.SensorBme280;
using RaspPiBme.Redis.RedisConfiguration;
using RaspPiBme.Mqtt;
using RaspPiBme;

namespace RaspPiBme.Services
{
    public class ServicesConfiguration : IServicesConfiguration
    {
        public IConfigurationRoot _configuration;
        //private ConfigureInitializationTimeSeries _configureInitializationTimeSeries;

        public ServicesConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath((AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IConfigurationRoot>(_configuration);
            serviceCollection.AddTransient<ConfigureInitializationTimeSeries>();
            serviceCollection.AddTransient<SensorBme280Configuration>();
            serviceCollection.AddTransient<ConfigurationRedisServer>();
            serviceCollection.AddTransient<MqttServerConfiguration>();
        }
    }
}

