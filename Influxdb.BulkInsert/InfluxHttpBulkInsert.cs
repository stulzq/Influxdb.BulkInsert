using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Influxdb.BulkInsert
{
    public class InfluxHttpBulkInsert: IInfluxBulkInsert,IDisposable
    {
        private readonly HttpClient _client;
        public InfluxHttpBulkInsert(InfluxConnectionSetting setting)
        {
            _client=new HttpClient(){BaseAddress = new Uri($"http://{setting.Server}:{setting.Server}")};
            BitchSize = setting.BitchSize;
        }

        public int BitchSize { get; }

        public async Task SendAsync(string data)
        {
            var stringContent = new StringContent(data);
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            var result = await _client.PostAsync("", stringContent);
            result.EnsureSuccessStatusCode();
            await result.Content.ReadAsStringAsync();
        }

        public async Task SendAsync(StringBuilder sb)
        {
            var stringContent = new StringContent(sb.ToString());
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            var result = await _client.PostAsync("", stringContent);
            result.EnsureSuccessStatusCode();
            await result.Content.ReadAsStringAsync();
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
