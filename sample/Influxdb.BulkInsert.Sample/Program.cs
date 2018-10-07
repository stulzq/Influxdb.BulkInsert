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
        private static Socket _client;
        static async Task  Main(string[] args)
        {
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var endPoint = new IPEndPoint(IPAddress.Parse("192.168.10.110"), 8088);
            _client.Connect(endPoint);
            ArraySegment<byte> a = Encoding.UTF8.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            await send(a);
            Console.WriteLine("Complete!");
            Console.ReadKey();
        }

        static async Task send(ArraySegment<byte> a)
        {
            await _client.SendAsync(a, SocketFlags.None);
        }
    }
}
