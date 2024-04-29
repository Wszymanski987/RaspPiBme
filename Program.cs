using System.Device.I2c;
using Iot.Device.Bmxx80;
using Microsoft.Extensions.DependencyInjection;
using RaspPiBme.Mqtt;
using RaspPiBme.Redis.ConfigureInitializationServices;
using RaspPiBme.Redis.RedisConfiguration;
using RaspPiBme.SensorBme280;
using RaspPiBme.Services;

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

Thread.Sleep(1000);

var i2cSettings = new I2cConnectionSettings(1, 118);

Thread.Sleep(1000);

using I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);

var sensorBme280Configuration = serviceProvider.GetService<SensorBme280Configuration>();

sensorBme280Configuration.Bme280 = new Bme280(i2cDevice);
int executionTimeSeconds = 5;

sensorBme280Configuration.StartMeasurements(executionTimeSeconds, configurationRedisServer._database);

await configurationRedisServer._database.StringSetAsync("measurement_complited", "true");