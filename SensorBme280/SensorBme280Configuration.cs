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
        public ServicesConfiguration ServicesConfiguration { get; set; }
        public int MeasurementTime { get; set; }
        public Bme280 Bme280 { get; set; }
        private ConfigureInitializationTimeSeries _configureInitializationTimeSeries;

        /*public SensorBme280Configuration(ConfigureInitializationTimeSeries configureInitializationTimeSeries)
        {
            //_servicesConfiguration = servicesConfiguration;
            //_configureInitializationTimeSeries = configureInitializationTimeSeries;
        }*/

        public void Initialize()
        {
            I2cConnectionCreate();

            MeasurementTime = Bme280.GetMeasurementDuration();
        }

        public void StartMeasurements(int executionTimeSeconds, IDatabase db)
        {
            int executionTimeMillis = executionTimeSeconds * 1000;
            int timer = 0;

            Console.WriteLine("Start Measurement");

            while (timer < 1000)
            {
                Console.Clear();

                Bme280.SetPowerMode(Bmx280PowerMode.Forced);
                //Thread.Sleep(MeasurementTime);
                Thread.Sleep(Bme280.GetMeasurementDuration());

                Bme280.TryReadTemperature(out var tempValue);
                Bme280.TryReadPressure(out var preValue);
                Bme280.TryReadHumidity(out var humValue);

                db.TimeSeriesAdd("ts_m:t:temp", "*", tempValue.DegreesCelsius);
                db.TimeSeriesAdd("ts_m:t:hum", "*", humValue.Percent);
                db.TimeSeriesAdd("ts_m:t:pres", "*", preValue.Hectopascals);

                Thread.Sleep(1000);

                timer += 100;
            }
        }


        private void I2cConnectionCreate() //to nie działa, bo using obowiązuje tylko do końca bloku?
        {
            Thread.Sleep(1000);
            //var address = int.Parse(_servicesConfiguration._configuration.GetSection("I2COptions:Address").Value);
            var i2cSettings = new I2cConnectionSettings(1, 118);

            using I2cDevice i2cDevice = I2cDevice.Create(i2cSettings);
            Bme280 = new Bme280(i2cDevice);
        }

        public void setTimeSeries(ConfigureInitializationTimeSeries configureInitializationTimeSeries)
        {
            _configureInitializationTimeSeries = configureInitializationTimeSeries;
        }

        public void Show()
        {
            Console.WriteLine("utworzono");
        }

    }
}

