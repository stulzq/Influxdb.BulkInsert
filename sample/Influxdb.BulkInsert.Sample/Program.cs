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
            RunHttp();
            RunUdp();

            Console.Read();
        }

        static void RunHttp()
        {
            var processor = new InfluxBulkInsertProcessor("Server=192.168.2.120;Port=8086;BitchSize=50;Database=kong;Timeout=30;",InfluxInsertProtocol.Http);

            processor.Open();
            for (int i = 0; i < 1000; i++)
            {
                processor.Write($"test,type='cpu' value=1 {InfluxdbTimeGen.GenNanosecond()}");
            }

            Console.WriteLine("Complete http");
            processor.Close();
        }

        static void RunUdp()
        {
            var processor = new InfluxBulkInsertProcessor("Server=192.168.2.120;Port=8089;BitchSize=10",InfluxInsertProtocol.Udp);

            processor.Open();
            for (int i = 0; i < 1000; i++)
            {
                processor.Write($"test,type='cpu' value={i} {InfluxdbTimeGen.GenNanosecond()}");
            }
            Console.WriteLine("Complete udp");
            processor.Close();
        }
    }
}
