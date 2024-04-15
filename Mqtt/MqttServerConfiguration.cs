using System;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using RaspPiBme.Services;

namespace RaspPiBme.Mqtt
{
    public class MqttServerConfiguration : IMqttServerConfiguration
    {
        private MqttFactory _mqttFactory;
        private ServicesConfiguration _servicesConfiguration;


        public MqttServerConfiguration(ServicesConfiguration servicesConfiguration)
        {
            _mqttFactory = new MqttFactory();
            _servicesConfiguration = servicesConfiguration;
        }

        public async Task MqttClientCreation()
        {
            var mqttClient = _mqttFactory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(_servicesConfiguration._configuration.GetSection("MqttOptions:Broker").Value)
                .WithClientId(_servicesConfiguration._configuration.GetSection("MqttOptions:ClientId").Value) //to nir może być na sztywno
                .WithCleanSession()
                .Build();


            var connectResult = await mqttClient.ConnectAsync(options);

            if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
            {
                //TODO
                //obsluga sybskrypcji 
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
            else
            {
                string errorMessage = $"Failed to connect to MQTT broker. Result code: {connectResult.ResultCode}";

                throw new Exception(errorMessage);
            }

        }
    }
}

