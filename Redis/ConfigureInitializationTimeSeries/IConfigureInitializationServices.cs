using System;
using StackExchange.Redis;

namespace RaspPiBme.redis.ConfigureInitializationServices
{
    public interface IConfigureInitializationServices
    {
        public void DeleteKeys(IDatabase db);

        public void InitializeTimeSeries(IDatabase db);

    }
}

