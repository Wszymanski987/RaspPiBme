using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Microsoft.Extensions.Configuration;
using static System.Console;

namespace RaspPiBme.Mqtt.Client.Configuration
{
    public class MqttClientConfiguration : IMqttClientConfiguration
    {
        private IMqttClient _mqttClient { get; set; }

        private List<string> _topics { get; set; }

        private IConfiguration _configuration { get; set; }

        private string _clientId { get; set; }

        private string _brokerAddress { get; set; }

        private int _port { get; set; }

        public MqttClientConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
            _clientId = _configuration.GetSection("MqttOptions:ClientId").Value;
            _brokerAddress = _configuration.GetSection("MqttOptions:Broker").Value;
            _port = Int32.Parse(_configuration.GetSection("MqttOptions:Port").Value);
        }

        public async Task CreateMqttClientAsync()
        {
            try
            {
                _mqttClient = new MqttFactory().CreateMqttClient();
            }
            catch (Exception ex)
            {
                WriteLine($"An error occurred in client creation: {ex.Message}");
            }
        }

        public async Task SendMqttMessageAsync(string topic, string payload)
        {
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtMostOnce)
                .WithRetainFlag()
                .Build();

            try
            {
                await EnsureMqttClientExistsAsync();
                await _mqttClient.PublishAsync(mqttMessage);
            }
            catch (Exception ex)
            {
                WriteLine($"sending message error {ex}");
            }
        }

        public async Task ClientConnectAsync()
        {
            var options = new MqttClientOptionsBuilder()
                                            .WithClientId(_clientId)
                                            .WithTcpServer(_brokerAddress, _port)
                                            .WithCleanSession()
                                            .Build();

            await EnsureMqttClientExistsAsync();

            try
            {
                await _mqttClient.ConnectAsync(options);
                WriteLine("Connected to MQTT broker successfully.");
            }
            catch (Exception ex)
            {
                WriteLine($"An error occurred, can't connect to MQTT broker: {ex.Message}");
            }
        }

        public async Task SubscribeMqttTopicAsync(string topic)
        {

            if (!_topics.Contains(topic))
            {
                _topics.Add(topic);

                await EnsureMqttClientExistsAsync();

                try
                {
                    await _mqttClient.SubscribeAsync(topic);
                    WriteLine($"Subscribed to topic: {topic}");
                }
                catch (Exception ex)
                {
                    WriteLine($"An error occurred during subscribing to topic: {ex.Message}");
                }
            }
            else
            {
                WriteLine($"Topic {topic} is already subscribed.");
            }
        }

        public async Task DisconnectFromMqttBroker()
        {
            await EnsureMqttClientExistsAsync();
            _mqttClient.DisconnectAsync();
        }

        private async Task EnsureMqttClientExistsAsync()
        {
            if (_mqttClient == null)
            {
                await CreateMqttClientAsync();
            }
        }
    }
}

