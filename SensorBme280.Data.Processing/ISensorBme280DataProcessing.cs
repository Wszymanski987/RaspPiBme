using StackExchange.Redis;

namespace RaspPiBme.SensorBme280.Data.Processing
{
    public interface ISensorBme280DataProcessing
    {
        Task<double> DataProcessAsync(IDatabase db, string timeSeriesKey);
    }
}