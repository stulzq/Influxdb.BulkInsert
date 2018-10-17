using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Influxdb.BulkInsert
{
    public class InfluxHttpBulkInsert : IInfluxBulkInsert, IDisposable
    {
        private readonly HttpClient _client;
        private readonly string _writePath;
        private readonly ILogger _logger;

        public InfluxHttpBulkInsert([NotNull] InfluxConnectionSetting setting)
        {
            if (string.IsNullOrEmpty(setting.Server))
            {
                throw new ArgumentException(nameof(setting.Server));
            }

            if (string.IsNullOrEmpty(setting.Database))
            {
                throw new ArgumentException(nameof(setting.Database));
            }

            if (setting.Port == 0)
            {
                throw new ArgumentException(nameof(setting.Port));
            }

            if (setting.BitchSize > 5000)
            {
                throw new ArgumentException("max BitchSize is 5000 form http protocol.", nameof(setting.BitchSize));
            }

            _logger = LogManager.GetLogger(this);
            _client = new HttpClient() {BaseAddress = new Uri($"http://{setting.Server}:{setting.Port}"),Timeout = TimeSpan.FromSeconds(setting.Timeout) };
            BitchSize = setting.BitchSize;
            if (string.IsNullOrEmpty(setting.UserName) && string.IsNullOrEmpty(setting.Password))
            {
                _writePath = $"/write?db={setting.Database}";
            }
            else
            {
                _writePath = $"/write?db={setting.Database}&u={setting.UserName}&p={setting.Password}";
            }

            _logger.LogInformation("Http bulk insert initialization success.");
        }

        public InfluxHttpBulkInsert([NotNull] string connectionString):this(new InfluxConnectionSetting(connectionString))
        {
        }

        public int BitchSize { get; }

        public async Task SendAsync(string data)
        {
            var stringContent = new StringContent(data);
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            var result = await _client.PostAsync(_writePath, stringContent);
            if ((int) result.StatusCode >= 400 || (int) result.StatusCode < 200)
            {
                _logger.LogError("Insert failure.Resp:\n"+ await result.Content.ReadAsStringAsync());
            }
            await result.Content.ReadAsStringAsync();
            _logger.LogDebug($"Send data success,Size {data.Length} character.");
        }

        public async Task SendAsync(StringBuilder sb)
        {
            await SendAsync(sb.ToString());
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}