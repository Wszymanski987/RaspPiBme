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
using static System.Console;

namespace RaspPiBme.SensorBme280
{
    public class SensorBme280Configuration : ISensorBme280Configuration
    {
        public ServicesConfiguration ServicesConfiguration { get; set; }
        public int MeasurementTime { get; set; }
        public Bme280 Bme280 { get; set; }

        public void Initialize()
        {
            try
            {
                I2cConnectionCreate();

                MeasurementTime = Bme280.GetMeasurementDuration();

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

            //wait db.StringSetAsync("measurement_complited", "false");

            while (cnt < measurementNumber)
            {
                Clear();

                Bme280.SetPowerMode(Bmx280PowerMode.Forced);
                await Task.Delay(MeasurementTime);

                Bme280.TryReadTemperature(out var tempValue);
                Bme280.TryReadPressure(out var preValue);
                Bme280.TryReadHumidity(out var humValue);

                await db.TimeSeriesAddAsync("ts_m:t:temp", "*", tempValue.DegreesCelsius);
                await db.TimeSeriesAddAsync("ts_m:t:hum", "*", humValue.Percent);
                await db.TimeSeriesAddAsync("ts_m:t:pres", "*", preValue.Hectopascals);

                await Task.Delay(1000);

                cnt++;
            }

            //await db.StringSetAsync("measurement_complited", "true");
        }

        private void I2cConnectionCreate()
        {
            Thread.Sleep(1000);
            //var address = int.Parse(_servicesConfiguration._configuration.GetSection("I2COptions:Address").Value);

            try
            {
                var i2cSettings = new I2cConnectionSettings(1, 118);

                I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
                Bme280 = new Bme280(i2cDevice);
            }
            catch (Exception ex)
            {
                WriteLine($"I2C error occurred: {ex.Message}");
            }
        }
    }
}

