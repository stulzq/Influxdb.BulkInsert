using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Influxdb.BulkInsert
{
    public class InfluxHealthCheck
    {
        private readonly HttpClient _client;
        private readonly ILogger _logger;

        public InfluxHealthCheck(InfluxConnectionSetting setting)
        {
            _logger = LogManager.GetLogger(this);
            _client = new HttpClient() {BaseAddress = new Uri($"http://{setting.Server}:{setting.Port}"),Timeout = TimeSpan.FromSeconds(setting.Timeout)};
        }

        public async Task<bool> Check()
        {
            try
            {
                var res = await _client.GetAsync("/status");
                res.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e,"Health check error.");
                return false;
            }
        }
    }
}