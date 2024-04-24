using System.Device.I2c;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Microsoft.Extensions.DependencyInjection;
using NRedisStack;
using NRedisTimeSeries;
using RaspPiBme.Mqtt;
using RaspPiBme.Redis.ConfigureInitializationServices;
using RaspPiBme.Redis.RedisConfiguration;
using RaspPiBme.SensorBme280;
using RaspPiBme.Services;
using StackExchange.Redis;

ServiceCollection serviceDescriptors = new ServiceCollection();

ServicesConfiguration servicesConfiguration = new ServicesConfiguration();
servicesConfiguration.ConfigureServices(serviceDescriptors);

IServiceProvider serviceProvider = serviceDescriptors.BuildServiceProvider();

//Redis
var configurationRedisServer = serviceProvider.GetService<ConfigurationRedisServer>();
configurationRedisServer._servicesConfiguration = servicesConfiguration;
configurationRedisServer.RedisConnectInitialization();

var configureInitializationTimeSeries = serviceProvider.GetRequiredService<ConfigureInitializationTimeSeries>();

//Mqtt
var configurationMqttServer = serviceProvider.GetService<MqttServerConfiguration>();
await configurationMqttServer.MqttClientCreation();

Console.WriteLine("mqtt");

//Console.WriteLine($"db:  {db.Database}");


//Sensor
//var bme280Sensor = serviceProvider.GetService<SensorBme280Configuration>();
//bme280Sensor.Initialize();
//bme280Sensor.setTimeSeries(configureInitializationTimeSeries);

//await bme280Sensor.StartMeasurements(5, configurationRedisServer._database);

Thread.Sleep(1000);
//var address = int.Parse(_servicesConfiguration._configuration.GetSection("I2COptions:Address").Value);
var i2cSettings = new I2cConnectionSettings(1, 118);

Thread.Sleep(1000);
using I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
var bme280 = new Bme280(i2cDevice);


var executionTimeSeconds = 5;

int executionTimeMillis = executionTimeSeconds * 1000;
int timer = 0;

while (timer < 1000)
{
    Console.Clear();

    bme280.SetPowerMode(Bmx280PowerMode.Forced);
    Thread.Sleep(bme280.GetMeasurementDuration());

    bme280.TryReadTemperature(out var tempValue);
    bme280.TryReadPressure(out var preValue);
    bme280.TryReadHumidity(out var humValue);

    Console.WriteLine($"{tempValue.DegreesCelsius}");

    configurationRedisServer._database.TimeSeriesAdd("ts_m:t:temp", "*", tempValue.DegreesCelsius);
    configurationRedisServer._database.TimeSeriesAdd("ts_m:t:hum", "*", humValue.Percent);
    configurationRedisServer._database.TimeSeriesAdd("ts_m:t:pres", "*", preValue.Hectopascals);

    Thread.Sleep(1000);

    timer += 100;
}












/*
//redis
ConfigurationRedisServer configurationRedisServer = new ConfigurationRedisServer(servicesConfiguration);
configurationRedisServer.RedisConnectInitialization();
ConfigureInitializationTimeSeries configureInitializationTimeSeries = new ConfigureInitializationTimeSeries(servicesConfiguration._configuration);


// MQTT
MqttServerConfiguration mqttServerConfiguration = new MqttServerConfiguration(servicesConfiguration);
await mqttServerConfiguration.MqttClientCreation();


//sensor
SensorBme280Configuration bme280Sensor = new SensorBme280Configuration(servicesConfiguration, configureInitializationTimeSeries);
//bme280Sensor.StartMeasurements(5, configurationRedisServer.);
//Console.WriteLine("dupa");*/

