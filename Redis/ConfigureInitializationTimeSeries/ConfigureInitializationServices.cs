using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NRedisTimeSeries;
using NRedisTimeSeries.DataTypes;
using NRedisTimeSeries.Commands;
using StackExchange.Redis;
using RaspPiBme.redis.ConfigureInitializationServices;

namespace RaspPiBme.Redis.ConfigureInitializationServices
{
    public class ConfigureInitializationTimeSeries : IConfigureInitializationServices
    {
        private readonly IConfigurationRoot _config;

         public ConfigureInitializationTimeSeries(IConfigurationRoot config)
         {
             _config = config;
         }

        public void DeleteKeys(IDatabase db)
        {
            try
            {
                //db.KeyDelete("ts_m:t"); //TimeSeries-Measurements-Total
                db.KeyDelete("ts_m:t:temp");//TimeSeries-Measurements-Total-Temperature
                db.KeyDelete("ts_m:t:hum");//TimeSeries-Measurements-Total-Humidity
                db.KeyDelete("ts_m:t:pres");//TimeSeries-Measurements-Total-Pressure
            }
            catch (Exception ex)
            {
                throw new Exception("Can't delete keys from database", ex);
            }

        }

        public void InitializeTimeSeries(IDatabase db)
        {
            try
            {
                var label = new TimeSeriesLabel("chart", "Measurementtype");
                var labels = new List<TimeSeriesLabel> { label };
                db.TimeSeriesCreate("ts_m:t:temp", retentionTime: 600000, labels: labels);
                db.TimeSeriesAdd("ts_m:t:temp", "*", 0);
                db.TimeSeriesCreate("ts_m:t:hum", retentionTime: 600000, labels: labels);
                db.TimeSeriesAdd("ts_m:t:hum", "*", 0);
                db.TimeSeriesCreate("ts_m:t:pres", retentionTime: 600000, labels: labels);
                db.TimeSeriesAdd("ts_m:t:pres", "*", 0);
            }
            catch (Exception ex)
            {
                throw new Exception("Can't create new Time Series", ex);
            }
        }


    }

}


