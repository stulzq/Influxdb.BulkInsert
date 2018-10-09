using System;

namespace Influxdb.BulkInsert
{
    public class InfluxdbTimeGen
    {
        private static Random rand=new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// Get the nanosecond local time zone
        /// </summary>
        /// <returns></returns>
        public static string GenNanosecond()
        {
            return $"{DateTimeOffset.Now.ToUnixTimeMilliseconds()}{ rand.Next(100000, 1000000)}";
        }

        /// <summary>
        /// Get the nanosecond specified time zone
        /// </summary>
        /// <param name="timeZone"></param>
        /// <returns></returns>
        public static string GenNanosecond(int timeZone)
        {
            return $"{DateTimeOffset.UtcNow.AddHours(timeZone).ToUnixTimeMilliseconds()}{ rand.Next(100000, 1000000)}";
        }
    }
}