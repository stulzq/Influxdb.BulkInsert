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
            var insert=new InfluxUdpBulkInsert(new InfluxConnectionSetting(){Server = "192.168.10.110",Port = 8089});
            var processor=new InfluxBulkInsertProcessor(insert);

            processor.Open();
            for (int i = 0; i < 1000; i++)
            {
                processor.Write($"test,type='cpu' value=1 {InfluxdbTimeGen.GenNanosecond()}");
            }
            
            Console.Read();
            processor.Close();
        }
    }
}
