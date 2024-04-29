using System.Device.I2c;
using Iot.Device.Bmxx80;
using Microsoft.Extensions.DependencyInjection;
using RaspPiBme.Mqtt;
using RaspPiBme.Redis.ConfigureInitializationServices;
using RaspPiBme.Redis.RedisConfiguration;
using RaspPiBme.SensorBme280;
using RaspPiBme.Services;
using static System.Console;

ServiceCollection serviceDescriptors = new ServiceCollection();

ServicesConfiguration servicesConfiguration = new ServicesConfiguration();
servicesConfiguration.ConfigureServices(serviceDescriptors);

IServiceProvider serviceProvider = serviceDescriptors.BuildServiceProvider();

//Redis
var configurationRedisServer = serviceProvider.GetService<ConfigurationRedisServer>();
configurationRedisServer._servicesConfiguration = servicesConfiguration;
configurationRedisServer.RedisConnectInitialization();

//Mqtt
var configurationMqttServer = serviceProvider.GetService<MqttServerConfiguration>();
await configurationMqttServer.MqttClientCreation();

var sensorBme280Configuration = serviceProvider.GetService<SensorBme280Configuration>();

try
{
    sensorBme280Configuration.Initialize();

    while(true) 
    {
        var measurementNumber = 10;
        await sensorBme280Configuration.StartMeasurements(measurementNumber, configurationRedisServer._database);

        await Task.Delay(TimeSpan.FromMinutes(5));
    }
}
catch (Exception ex)
{
    WriteLine($"An error occurred: {ex.Message}");
}