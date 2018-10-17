using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Influxdb.BulkInsert
{
    public class InfluxUdpBulkInsert: IInfluxBulkInsert,IDisposable
    {
        private readonly Socket _client;
        private readonly ILogger _logger;

        public InfluxUdpBulkInsert([NotNull]InfluxConnectionSetting setting)
        {
            if (string.IsNullOrEmpty(setting.Server))
            {
                throw new ArgumentException(nameof(setting.Server));
            }

            if (setting.Port == 0)
            {
                throw new ArgumentException(nameof(setting.Port));
            }

            _client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var endPoint = new IPEndPoint(IPAddress.Parse(setting.Server), setting.Port);
            _client.Connect(endPoint);
            BitchSize = setting.BitchSize;
            _logger = LogManager.GetLogger(this);
            _logger.LogInformation("Udp bulk insert initialization success.");
        }

        public InfluxUdpBulkInsert([NotNull] string connectionString):this(new InfluxConnectionSetting(connectionString))
        {
        }

        public int BitchSize { get; }

        public async Task SendAsync(string data)
        {
            ArraySegment<byte> bytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(data));
            await _client.SendAsync(bytes, SocketFlags.None);
            _logger.LogDebug($"Send data success,Size {data.Length} character.");
        }

        public async Task SendAsync(StringBuilder sb)
        {
            await SendAsync(sb.ToString());
        }

        public void Dispose()
        {
            _client?.Close();
            _client?.Dispose();
        }

    }
}