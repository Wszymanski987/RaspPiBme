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

        //ServicesConfiguration servicesConfiguration to było w konstruktorze
        public MqttServerConfiguration()
        {
            _mqttFactory = new MqttFactory();
        }

        public async Task MqttClientCreation()
        {
            _mqttClient = _mqttFactory.CreateMqttClient();
            //var tcpServerValue = _servicesConfiguration._configuration.GetSection("MqttOptions:Broker").Value;
            //var clientIdValue = _servicesConfiguration._configuration.GetSection("MqttOptions:ClientId").Value;

            //if (tcpServerValue != null && clientIdValue != null)
            {
                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer("192.168.0.248")
                    .WithClientId("sensorBme280") //to nie może być na sztywno
                    .WithCleanSession()
                    .Build();

                var connectResult = await _mqttClient.ConnectAsync(options);

                if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
                {
                    Console.WriteLine("Connected to MQTT broker successfully.");

                    //TODO
                    //obsluga sybskrypcji 
                    // Subscribe to a topic
                    await _mqttClient.SubscribeAsync("cmnd/humidity/POWER");

                    // Callback function when a message is received
                    _mqttClient.ApplicationMessageReceivedAsync += e =>
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
            /*else
            {
                Console.WriteLine("Broker or client ID configuration value is null or empty.");
            }*/

        }
    }
}

