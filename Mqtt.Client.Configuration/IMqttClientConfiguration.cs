using System;
namespace RaspPiBme.Mqtt.Client.Configuration
{
    public interface IMqttClientConfiguration
    {
        Task CreateMqttClientAsync();

        Task SendMqttMessageAsync(string topic, string payload);

        Task ClientConnectAsync();

        Task SubscribeMqttTopicAsync(string topic);

        Task DisconnectFromMqttBroker();
    }
}

