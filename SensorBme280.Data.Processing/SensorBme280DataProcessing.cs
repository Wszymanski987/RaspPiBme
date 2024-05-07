using NRedisTimeSeries;
using StackExchange.Redis;
using static System.Console;

namespace RaspPiBme.SensorBme280.Data.Processing
{
    public class SensorBme280DataProcessing : ISensorBme280DataProcessing
    {
        public double avg { get; set; }
        public async Task<double> DataProcessAsync(IDatabase db, string timeSeriesKey)
        {
            if (db != null)
            {
                var key = timeSeriesKey;
                var range = await db.TimeSeriesRangeAsync(key, "-", "+");

                if (range != null && range.Any())
                {
                    double sum = 0;
                    int cnt = 0;
                    foreach (var point in range)
                    {
                        sum += point.Val;
                        cnt++;
                    }

                    avg = sum / cnt;
                    WriteLine($"Avarage value for Time Serie {timeSeriesKey} is: {avg}");

                    return avg;
                }
                else throw new Exception("Time Serie is empty");
            }
            else throw new Exception("There is nbo data in database");
        }
    }
}