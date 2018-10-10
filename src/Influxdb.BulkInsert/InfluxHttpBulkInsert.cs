using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Influxdb.BulkInsert
{
    public class InfluxHttpBulkInsert : IInfluxBulkInsert, IDisposable
    {
        private readonly HttpClient _client;
        private readonly string _writePath;

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

            _client = new HttpClient() {BaseAddress = new Uri($"http://{setting.Server}:{setting.Port}")};
            BitchSize = setting.BitchSize;
            if (string.IsNullOrEmpty(setting.UserName) && string.IsNullOrEmpty(setting.Password))
            {
                _writePath = $"/write?db={setting.Database}";
            }
            else
            {
                _writePath = $"/write?db={setting.Database}&u={setting.UserName}&p={setting.Password}";
            }
        }

        public int BitchSize { get; }

        public async Task SendAsync(string data)
        {
            var stringContent = new StringContent(data);
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            var result = await _client.PostAsync(_writePath, stringContent);
            result.EnsureSuccessStatusCode();
            await result.Content.ReadAsStringAsync();
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