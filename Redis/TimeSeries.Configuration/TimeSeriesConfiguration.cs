using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NRedisTimeSeries;
using NRedisTimeSeries.DataTypes;
using NRedisTimeSeries.Commands;
using StackExchange.Redis;

namespace RaspPiBme.Redis.TimeSeries.Configuration
{
    public class TimeSeriesConfiguration : ITimeSeriesConfiguration
    {
        private readonly IConfiguration _configuration;
        

        public TimeSeriesConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void DeleteKeys(IDatabase db)
        {
            if (db != null)
            {
                try
                {
                    db.KeyDelete("ts_m:t:hum");
                    db.KeyDelete("ts_m:t:temp");
                    db.KeyDelete("ts_m:t:press");
                }
                catch (Exception ex)
                {
                    throw new Exception("Can't delete keys from database", ex);
                }
            }
            else throw new Exception("There is no data in database");

        }

        public void InitializeTimeSeries(IDatabase db)
        {
            if (db != null)
            {
                try
                {
                    var label = new TimeSeriesLabel("chart", "MeasurementType");
                    var labels = new List<TimeSeriesLabel> { label };
                    db.TimeSeriesCreate("ts_m:t:temp", retentionTime: 600000, labels: labels);
                    db.TimeSeriesAdd("ts_m:t:temp", "*", 0);
                    db.TimeSeriesCreate("ts_m:t:hum", retentionTime: 600000, labels: labels);
                    db.TimeSeriesAdd("ts_m:t:hum", "*", 0);
                    db.TimeSeriesCreate("ts_m:t:press",
                        retentionTime: 600000, labels: labels);
                    db.TimeSeriesAdd("ts_m:t:press", "*", 0);
                }
                catch (Exception ex)
                {
                    throw new Exception("Can't create new Time Series", ex);
                }
            }
            else throw new Exception("There is no data in database");
        }


    }

}


