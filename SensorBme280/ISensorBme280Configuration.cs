using System;
using StackExchange.Redis;

namespace RaspPiBme.SensorBme280
{
    public interface ISensorBme280Configuration
    {
        public void Initialize();

        public Task StartMeasurements(int executionTimeSeconds, IDatabase db);
    }
}

