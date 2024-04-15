using System;
namespace RaspPiBme.Mqtt
{
    public interface IMqttServerConfiguration
    {
        public Task MqttClientCreation();
    }
}

