using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RaspPiBme.Redis.TimeSeries.Configuration;
using RaspPiBme.SensorBme280.Data.Processing;
using RaspPiBme.Redis.RedisServer.Configuration;
using RaspPiBme.Mqtt.Client.Configuration;
using RaspPiBme.SonoffBasic.Controller;
using RaspPiBme.SensorBme280.Configuration;

namespace RaspPiBme.Services.Configuration
{
    public class ServicesConfiguration : IServicesConfiguration
    {
        public IConfiguration _configuration;

        public ServicesConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath((AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IConfiguration>(_configuration);
            serviceCollection.AddTransient<TimeSeriesConfiguration>();
            serviceCollection.AddTransient<SensorBme280Configuration>();
            serviceCollection.AddTransient<RedisServerConfiguration>();
            serviceCollection.AddTransient<MqttClientConfiguration>();
            serviceCollection.AddTransient<SensorBme280DataProcessing>();
            serviceCollection.AddTransient<SonoffBasicController>();
        }
    }
}

