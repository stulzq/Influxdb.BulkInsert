using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Influxdb.BulkInsert
{
    public class InfluxBulkInsertProcessor
    {
        private readonly IInfluxBulkInsert _bulkInsert;
        private bool _running;
        private bool _stop;
        private readonly ConcurrentQueue<string> _queue=new ConcurrentQueue<string>();
        private readonly ILogger _logger;
        private readonly InfluxHealthCheck _healthCheck;
        private readonly object _lockObject=new object();
        private bool _stopEqueue;
        private const int MAXQUEUECOUNT=1000000;

        public InfluxBulkInsertProcessor(InfluxConnectionSetting setting, InfluxInsertProtocol protocol)
        {
            _logger = LogManager.GetLogger(this);
            if (protocol == InfluxInsertProtocol.Http)
            {
                _bulkInsert = new InfluxHttpBulkInsert(setting);
                _logger.LogInformation("InfluxBulkInsertProcessor use http.");
            }
            else
            {
                _bulkInsert = new InfluxUdpBulkInsert(setting);
                _logger.LogInformation("InfluxBulkInsertProcessor use udp.");
            }
            _healthCheck=new InfluxHealthCheck(setting);
        }

        public InfluxBulkInsertProcessor(string connectionString, InfluxInsertProtocol protocol):this(new InfluxConnectionSetting(connectionString),protocol)
        {
            
        }

        public void Write(string data)
        {
            if (_stopEqueue || _queue.Count >= MAXQUEUECOUNT)
            {
                return;
            }
            _queue.Enqueue(data);
        }

        public void Open()
        {
            lock (_lockObject)
            {
                if (!_running)
                {
                    _running = true;
                    Task.Run(Work);
                }
            }
        }

        private async Task Work()
        {
            _logger.LogInformation( "Processor start.");
            while (_running|| _queue.Count>0)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    var count = _queue.Count >= _bulkInsert.BitchSize ? _bulkInsert.BitchSize : _queue.Count;
                    for (int i = 0; i < count - 1; i++)
                    {
                        if (_queue.TryDequeue(out var data))
                        {
                            sb.Append($"{data}\n");
                        }
                    }

                    //Reduce inner loop judgment
                    if (_queue.TryDequeue(out var lastData))
                    {
                        sb.Append(lastData);
                    }

                    if (sb.Length > 0)
                    {
                        await _bulkInsert.SendAsync(sb);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }

                    if (!_stopEqueue) continue;
                    if (await _healthCheck.Check())
                    {
                        _stopEqueue = false;
                    }
                }
                catch (Exception e)
                {
                    _stopEqueue=true;
                    _logger.LogError(e, "Error Occurred.");
                }
            }

            _stop = true;

        }

        public void Close()
        {
            _running = false;
            while (!_stop) { Thread.Sleep(200);}
        }
    }
}