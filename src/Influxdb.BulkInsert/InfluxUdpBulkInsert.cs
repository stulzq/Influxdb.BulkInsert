using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Influxdb.BulkInsert
{
    public class InfluxUdpBulkInsert: IInfluxBulkInsert,IDisposable
    {
        private readonly Socket _client;
        private readonly IPEndPoint endPoint;
        public InfluxUdpBulkInsert(InfluxConnectionSetting setting)
        {
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            endPoint = new IPEndPoint(IPAddress.Parse(setting.Server), setting.Port);
            _client.Connect(endPoint);
            BitchSize = setting.BitchSize;
        }

        public int BitchSize { get; }

        public async Task SendAsync(string data)
        {
            ArraySegment<byte> bytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(data));
            await _client.SendAsync(bytes, SocketFlags.None);
            
        }

        public async Task SendAsync(StringBuilder sb)
        {
            ArraySegment<byte> bytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(sb.ToString()));
            await _client.SendAsync(bytes, SocketFlags.None);
            Console.WriteLine("发送");
        }

        public void Dispose()
        {
            _client?.Close();
            _client?.Dispose();
        }

    }
}