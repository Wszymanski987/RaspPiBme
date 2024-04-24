using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Formatter.V3;
using RaspPiBme.Services;

namespace RaspPiBme.Mqtt
{
    public class MqttServerConfiguration : IMqttServerConfiguration
    {
        private readonly MqttFactory _mqttFactory;
        private IMqttClient? _mqttClient;

        public MqttServerConfiguration()
        {
            _mqttFactory = new MqttFactory();
        }

        public async Task MqttClientCreation()
        {
            _mqttClient = _mqttFactory.CreateMqttClient();

            {
                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer("192.168.0.248")
                    .WithClientId("sensorBme280")
                    .WithCleanSession()
                    .Build();

                var connectResult = await _mqttClient.ConnectAsync(options);

                if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
                {
                    Console.WriteLine("Connected to MQTT broker successfully.");
                }
                else
                {
                    string errorMessage = $"Failed to connect to MQTT broker. Result code: {connectResult.ResultCode}";

                    throw new Exception(errorMessage);
                }
            }
        }
    }
}

