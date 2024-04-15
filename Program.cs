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
using System.Net;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NRedisTimeSeries;


//redis
IConfigurationRoot configuration;
ServiceCollection serviceDescriptors = new ServiceCollection();

ConfigureServices(serviceDescriptors);
IServiceProvider serviceProvider = serviceDescriptors.BuildServiceProvider();

var redisOptions = new ConfigurationOptions
{
    EndPoints = { "redis-11448.c311.eu-central-1-1.ec2.cloud.redislabs.com:11448" },
    Password = "FalHPHe0ItdVvgdG7iS9BwXqTa50Dvox",
    Ssl = false
};

ConnectionMultiplexer redisMultiplexer = ConnectionMultiplexer.Connect(redisOptions);

if (redisMultiplexer.IsConnecting)
    Console.WriteLine("Redis");


IDatabase db = redisMultiplexer.GetDatabase();


// MQTT
var factory = new MqttFactory();
var mqttClient = factory.CreateMqttClient();

var options = new MqttClientOptionsBuilder()
    .WithTcpServer("192.168.0.248")
    .WithClientId("sensorBme280")
    .WithCleanSession()
    .Build();

var connectResult = await mqttClient.ConnectAsync(options);

if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
{
    Console.WriteLine("Connected to MQTT broker successfully.");

    // Subscribe to a topic
    await mqttClient.SubscribeAsync("cmnd/humidity/POWER");

    // Callback function when a message is received
    mqttClient.ApplicationMessageReceivedAsync += e =>
    {
        Console.WriteLine($"Received message: {Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)}");
        return Task.CompletedTask;
    };
}



//sensor

Thread.Sleep(1000);
var Address = configuration.GetSection("I2COptions:Address").Value;
var i2cSettings = new I2cConnectionSettings(1, 118);

using I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
using var bme280 = new Bme280(i2cDevice);

int measurementTime = bme280.GetMeasurementDuration();

int executionTime = 300000;
int timer = 0;

while (timer < executionTime)
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

    db.TimeSeriesAdd("ts_m:t:temp", "*", tempValue.DegreesCelsius);
    db.TimeSeriesAdd("ts_m:t:hum", "*", humValue.Percent);
    db.TimeSeriesAdd("ts_m:t:pres", "*", preValue.Hectopascals);

    Thread.Sleep(1000);
    timer++;
}


void ConfigureServices(IServiceCollection serviceCollection)
{
    configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
        .AddJsonFile("appsettings.json")
        .Build();

    serviceCollection.AddSingleton<IConfigurationRoot>(configuration);
    serviceCollection.AddTransient<ConfigureInitializationServices>();
    //serviceCollection.AddTransient<DataServices>();

}