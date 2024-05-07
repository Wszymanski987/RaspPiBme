using System;
using StackExchange.Redis;

namespace RaspPiBme.Redis.TimeSeries.Configuration
{
    public interface ITimeSeriesConfiguration
    {
        public void DeleteKeys(IDatabase db);

        public void InitializeTimeSeries(IDatabase db);

    }
}

