using System.Device.I2c;
using Iot.Device.Bmxx80;
using StackExchange.Redis;
using NRedisTimeSeries;
using static System.Console;
using Microsoft.Extensions.Configuration;
using Iot.Device.Bmxx80.PowerMode;

namespace RaspPiBme.SensorBme280.Configuration
{
    public class SensorBme280Configuration : ISensorBme280Configuration
    {
        public Bme280 SensorBme280 { get; set; }
        
        private int MeasurementTime { get; set; }

        private IConfiguration _configuration { get; set; }

        private string _timeSerieHumidity { get; set; }

        private string _timeSerieTemperature { get; set; }

        private string _timeSeriePressure { get; set; }

        private int _i2cAddress { get; set; }

        public SensorBme280Configuration(IConfiguration configuration)
        {
            _configuration = configuration;
            _timeSerieHumidity = _configuration.GetSection("Humidity:TimeSerie").Value;
            _timeSerieTemperature = _configuration.GetSection("Temperature:TimeSerie").Value;
            _timeSeriePressure = _configuration.GetSection("Pressure:TimeSerie").Value;
            _i2cAddress = Int32.Parse(_configuration.GetSection("I2COptions:Address").Value);
        }

        public void Initialize()
        {
            try
            {
                I2cConnectionCreate();

                MeasurementTime = SensorBme280.GetMeasurementDuration();
            }
            catch (Exception ex)
            {
                WriteLine($"Initialization error occurred: {ex.Message}");
            }
        }

        public async Task StartMeasurements(int measurementNumber, IDatabase db)
        {
            Thread.Sleep(1000);

            var cnt = 0;

            while (cnt < measurementNumber)
            {
                Clear();

                SensorBme280.SetPowerMode(Bmx280PowerMode.Forced);
                await Task.Delay(MeasurementTime);

                SensorBme280.TryReadTemperature(out var tempValue);
                SensorBme280.TryReadPressure(out var preValue);
                SensorBme280.TryReadHumidity(out var humValue);

                WriteLine($"hum value {humValue.Percent}");
                WriteLine($"temp value {tempValue.DegreesCelsius}");

                await db.TimeSeriesAddAsync("ts_m:t:temp", "*", tempValue.DegreesCelsius);
                await db.TimeSeriesAddAsync("ts_m:t:hum", "*", humValue.Percent);
                await db.TimeSeriesAddAsync("ts_m:t:press", "*", preValue.Hectopascals);


                await Task.Delay(1000);

                cnt++;
            }
        }

        private void I2cConnectionCreate()
        {
            Thread.Sleep(1000);

            try
            {  
                var i2cSettings = new I2cConnectionSettings(1, 118);

                I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
                SensorBme280 = new Bme280(i2cDevice);
            }
            catch (Exception ex)
            {
                WriteLine($"I2C error occurred: {ex.Message}");
            }
        }
    }
}

