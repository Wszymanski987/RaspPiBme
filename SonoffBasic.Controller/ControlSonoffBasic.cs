using Microsoft.Extensions.Configuration;
using MQTTnet;

namespace RaspPiBme.SonoffBasic.Controller
{
    public class SonoffBasicController : ISonoffBasicController
    {
        private MqttApplicationMessageBuilder? _mqttMessage { get; set; }

        private IConfiguration? _configuration { get; set; }

        public string? Topic { get; set; }

        public string? Payload { get; set; }

        private string _timeSerieHumidity { get; set; }

        private string _timeSerieTemperature { get; set; }

        public SonoffBasicController(IConfiguration configuration)
        {
            _configuration = configuration;
            _timeSerieHumidity = _configuration.GetSection("Humidity:TimeSerie").Value;
            _timeSerieTemperature = _configuration.GetSection("Temperature:TimeSerie").Value;
        }

        public void SwitchSonoffBasic(double avg, string measurementType)
        {
            if (string.Equals(measurementType, "humidity", StringComparison.OrdinalIgnoreCase))
            {
                HumiditySonoffControl(avg);
            }
            else if (string.Equals(measurementType, "temperature", StringComparison.OrdinalIgnoreCase))
            {
                TemperatureSonoffControl(avg);
            }
            else throw new Exception("The measurement type doesn't exist");
        }

        private async Task HumiditySonoffControl(double avg)
        {
            if (avg >= 0 && avg <= 100)
            {
                if (60 > avg)
                {
                    Topic = "cmnd/" + "ts_m:t:hum" + "/POWER";
                    Payload = "ON";
                }
                else
                {
                    Topic = "cmnd/" + "ts_m:t:hum" + "/POWER";
                    Payload = "OFF";
                }
            } else throw new ArgumentException("The avarage humidity is out of range");
        }

        private async Task TemperatureSonoffControl(double avg)
        {
            if (avg >= -50 && avg <= 50)
            {

                if (20 < avg)
                {
                    Topic = "cmnd/" + "ts_m:t:temp" + "/POWER";
                    Payload = "ON";
                }
                else
                {
                    Topic = "cmnd/" + "ts_m:t:temp" + "/POWER";
                    Payload = "OFF";
                }
            } else throw new ArgumentException("The avarage temperature is out of range"); 
        }
    }
}