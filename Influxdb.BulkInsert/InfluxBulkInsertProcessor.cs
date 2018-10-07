using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace Influxdb.BulkInsert
{
    public class InfluxBulkInsertProcessor
    {
        private readonly IInfluxBulkInsert _bulkInsert;
        private bool _running=true;
        private bool _stop;
        private readonly Thread _workThread;
        private readonly ConcurrentQueue<string> _queue=new ConcurrentQueue<string>();

        public InfluxBulkInsertProcessor(IInfluxBulkInsert bulkInsert)
        {
            _bulkInsert = bulkInsert;
            _workThread = new Thread(Work) {IsBackground = true};
            _workThread.Start();
        }

        public void Write(string data)
        {
            _queue.Enqueue(data);
        }

        private void Work()
        {
            StringBuilder sb = new StringBuilder();
            while (_running)
            {
                try
                {
                    var count = _queue.Count >= _bulkInsert.BitchSize ? _bulkInsert.BitchSize : _queue.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (_queue.TryDequeue(out var data))
                        {

                            sb.AppendLine(data);
                        }
                    }

                    _bulkInsert.SendAsync(sb);

                    if (count < _bulkInsert.BitchSize)
                    {
                        Thread.Sleep(1000);
                    }

                    sb.Clear();
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