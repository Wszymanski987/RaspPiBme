using Microsoft.Extensions.DependencyInjection;
using RaspPiBme.Redis.TimeSeries.Configuration;
using RaspPiBme.SensorBme280.Data.Processing;
using RaspPiBme.Redis.RedisServer.Configuration;
using RaspPiBme.Mqtt.Client.Configuration;
using RaspPiBme.SonoffBasic.Controller;
using static System.Console;
using RaspPiBme.Services.Configuration;
using RaspPiBme.SensorBme280.Configuration;

ServiceCollection serviceDescriptors = new ServiceCollection();

ServicesConfiguration servicesConfiguration = new ServicesConfiguration();
servicesConfiguration.ConfigureServices(serviceDescriptors);

IServiceProvider serviceProvider = serviceDescriptors.BuildServiceProvider();

//Redis
var redisServerConfiguration = serviceProvider.GetService<RedisServerConfiguration>();
redisServerConfiguration.RedisConnectInitialization();

//Mqtt
var mqttClientConfiguration = serviceProvider.GetService<MqttClientConfiguration>();
await mqttClientConfiguration.CreateMqttClientAsync();

var sensorBme280Configuration = serviceProvider.GetService<SensorBme280Configuration>();

var sensorBme280DataProcessing = serviceProvider.GetService<SensorBme280DataProcessing>(); 

var sonoffBasicController = serviceProvider.GetService<SonoffBasicController>();

try
{
    mqttClientConfiguration.ClientConnectAsync();
    mqttClientConfiguration.SubscribeMqttTopicAsync("cmnd/ts_m:t:hum/POWER");
    mqttClientConfiguration.SubscribeMqttTopicAsync("cmnd/ts_m:t:temp/POWER");

    sensorBme280Configuration.Initialize();

    while(true) 
    {
        var measurementNumber = 10;
        await sensorBme280Configuration.StartMeasurements(measurementNumber, redisServerConfiguration.Database);

        await Task.Delay(TimeSpan.FromMinutes(1));

        await sensorBme280DataProcessing.DataProcessAsync(redisServerConfiguration.Database, "ts_m:t:hum");
        sonoffBasicController.SwitchSonoffBasic(sensorBme280DataProcessing.avg, "humidity");
        await mqttClientConfiguration.SendMqttMessageAsync(sonoffBasicController.Topic, sonoffBasicController.Payload);

        await sensorBme280DataProcessing.DataProcessAsync(redisServerConfiguration.Database, "ts_m:t:temp");
        sonoffBasicController.SwitchSonoffBasic(sensorBme280DataProcessing.avg, "temperature");
        await mqttClientConfiguration.SendMqttMessageAsync(sonoffBasicController.Topic, sonoffBasicController.Payload);
    }
}
catch (Exception ex)
{
    WriteLine($"An error occurred: {ex.Message}");
}