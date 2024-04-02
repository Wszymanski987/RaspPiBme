using System;
using System.Device.I2c;
using System.Threading;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using MQTTnet.Server;
using MQTTnet;
using System.Text;
using MQTTnet.Client;
using MQTTnet.Protocol;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

IConfigurationRoot configuration;
ServiceCollection serviceDescriptors = new ServiceCollection();

ConfigureServices(serviceDescriptors);
IServiceProvider serviceProvider = serviceDescriptors.BuildServiceProvider();

//redis

var redisOptions = new ConfigurationOptions
{
    EndPoints = configuration.GetSection("RedisOptions:EndPoints").Value,
    Password = configuration.GetSection("RedisOptions:Password").Value,
    Ssl = false
};

ConnectionMultiplexer redisMultiplexer = ConnectionMultiplexer.Connect(redisOptions);
IDatabase db = redisMultiplexer.GetDatabase();


// MQTT
var factory = new MqttFactory();
var mqttClient = factory.CreateMqttClient();

var options = new MqttClientOptionsBuilder()
    .WithTcpServer(configuration.GetSection("MqttOptions:Broker").Value)
    .WithTcpServer(configuration.GetSection("MqttOptions:Port").Value)
    .WithClientId(configuration.GetSection("MqttOptions:clientId").Value)
    .WithCleanSession()
    .Build();

var connectResult = await mqttClient.ConnectAsync(options);

if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
{
    Console.WriteLine("Connected to MQTT broker successfully.");

    await mqttClient.SubscribeAsync(configuration.GetSection("MqttOptions:Topic").Value);

    mqttClient.ApplicationMessageReceivedAsync += e =>
    {
        Console.WriteLine($"Received message: {Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)}");
        return Task.CompletedTask;
    };
}

//sensor

Thread.Sleep(1000);
var i2cSettings = new I2cConnectionSettings(1, 118);

using I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
using var bme280 = new Bme280(i2cDevice);

int measurementTime = bme280.GetMeasurementDuration();


while (true)
{
    Console.Clear();

    bme280.SetPowerMode(Bmx280PowerMode.Forced);
    Thread.Sleep(measurementTime);

    bme280.TryReadTemperature(out var tempValue);
    bme280.TryReadPressure(out var preValue);
    bme280.TryReadHumidity(out var humValue);

    Console.WriteLine($"Temperature: {tempValue.DegreesCelsius:0.#}\u00B0C");
    Console.WriteLine($"Pressure: {preValue.Hectopascals:#.##} hPa");
    Console.WriteLine($"Relative humidity: {humValue.Percent:#.##}%");

    //await mqttClient.PublishAsync(messageTemp);
}

Thread.Sleep(1000);


void ConfigureServices(IServiceCollection serviceCollection)
{
    IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
        .AddJsonFile("appsettings.json")
        .Build();

    serviceCollection.AddSingleton<IConfigurationRoot>(configuration);
    serviceCollection.AddTransient<ConfigureInitializationServices>();
}