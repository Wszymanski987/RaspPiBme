using RaspPiBme.Mqtt;
using RaspPiBme.Redis.ConfigureInitializationServices;
using RaspPiBme.Redis.RedisConfiguration;
using RaspPiBme.Services;

ServicesConfiguration servicesConfiguration = new ServicesConfiguration();

//redis
ConfigurationRedisServer configurationRedisServer = new ConfigurationRedisServer(servicesConfiguration);
ConfigureInitializationTimeSeries configureInitializationTimeSeries = new ConfigureInitializationTimeSeries(servicesConfiguration._configuration);
// MQTT
MqttServerConfiguration mqttServerConfiguration = new MqttServerConfiguration(servicesConfiguration);
await mqttServerConfiguration.MqttClientCreation();


//sensor
//SensorBme280Configuration bme280Sensor = new SensorBme280Configuration(servicesConfiguration, configureInitializationTimeSeries);

Console.WriteLine("dupa");

