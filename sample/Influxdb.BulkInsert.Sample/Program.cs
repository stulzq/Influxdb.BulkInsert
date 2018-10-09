using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Influxdb.BulkInsert.Sample
{
    class Program
    {
        static void  Main(string[] args)
        {
            var insert=new InfluxUdpBulkInsert(new InfluxConnectionSetting(){Server = "127.0.0.1",Port = 8088});
            var processor=new InfluxBulkInsertProcessor(insert);
            processor.Write("test,type=cpu value=50.0");
            Console.ReadKey();
        }
    }
}
