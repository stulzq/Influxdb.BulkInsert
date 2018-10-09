using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Influxdb.BulkInsert
{
    public class InfluxBulkInsertProcessor
    {
        private readonly IInfluxBulkInsert _bulkInsert;
        private bool _running;
        private bool _stop;
        private readonly ConcurrentQueue<string> _queue=new ConcurrentQueue<string>();

        public InfluxBulkInsertProcessor(IInfluxBulkInsert bulkInsert)
        {
            _bulkInsert = bulkInsert;
        }

        public void Write(string data)
        {
            _queue.Enqueue(data);
        }

        public void Open()
        {
            _running = true;
            Task.Run(Work);
        }

        private async Task Work()
        {
            
            while (_running|| _queue.Count>0)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    var count = _queue.Count >= _bulkInsert.BitchSize ? _bulkInsert.BitchSize : _queue.Count;
                    for (int i = 0; i < count-1; i++)
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
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
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