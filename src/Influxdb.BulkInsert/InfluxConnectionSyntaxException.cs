using System;

namespace Influxdb.BulkInsert
{
    public class InfluxConnectionSyntaxException:Exception
    {
        public InfluxConnectionSyntaxException(string message):base(message)
        {
            
        }

        public InfluxConnectionSyntaxException(string message,Exception inner) : base(message, inner)
        {

        }
    }
}