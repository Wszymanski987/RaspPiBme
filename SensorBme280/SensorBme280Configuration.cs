using System;
using System.Device.I2c;
using Iot.Device.Bmxx80;
using System.Threading;
using Iot.Device.Bmxx80.PowerMode;
using Iot.Device.Card.Ultralight;
using RaspPiBme.Redis.ConfigureInitializationServices;
using StackExchange.Redis;
using NRedisTimeSeries;
using RaspPiBme.Services;

namespace RaspPiBme.SensorBme280
{
    public class SensorBme280Configuration : ISensorBme280Configuration
    {
        private ServicesConfiguration _servicesConfiguration;
        private int _measurementTime;
        private Bme280 _bme280;
        private ConfigureInitializationTimeSeries _configureInitializationTimeSeries;

        public SensorBme280Configuration(ServicesConfiguration servicesConfiguration, ConfigureInitializationTimeSeries configureInitializationTimeSeries)
        {
            _servicesConfiguration = servicesConfiguration;
            _configureInitializationTimeSeries = configureInitializationTimeSeries;
        }

        public void Initialize()
        {
            I2cConnectionCreate();

            _measurementTime = _bme280.GetMeasurementDuration();
        }

        public void StartMeasurements(int executionTimeSeconds, IDatabase db)
        {
            int executionTimeMillis = executionTimeSeconds * 1000;
            int timer = 0;

            while (timer < executionTimeMillis)
            {
                Console.Clear();

                _bme280.SetPowerMode(Bmx280PowerMode.Forced);
                Thread.Sleep(_measurementTime);

                _bme280.TryReadTemperature(out var tempValue);
                _bme280.TryReadPressure(out var preValue);
                _bme280.TryReadHumidity(out var humValue);

                db.TimeSeriesAdd("ts_m:t:temp", "*", tempValue.DegreesCelsius);
                db.TimeSeriesAdd("ts_m:t:hum", "*", humValue.Percent);
                db.TimeSeriesAdd("ts_m:t:pres", "*", preValue.Hectopascals);

                Thread.Sleep(1000);

                timer += 1000;
            }
        }


        private void I2cConnectionCreate()
        {
            Thread.Sleep(1000);
            var address = int.Parse(_servicesConfiguration._configuration.GetSection("I2COptions:Address").Value);
            var i2cSettings = new I2cConnectionSettings(1, address);

            using I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
            _bme280 = new Bme280(i2cDevice);
        }

    }
}

