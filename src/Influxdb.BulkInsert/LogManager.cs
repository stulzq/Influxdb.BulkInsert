using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Influxdb.BulkInsert
{
    public class LogManager
    {
        private static ILoggerFactory _loggerFactory=new NullLoggerFactory();
        public  static void Init(ILoggerFactory factory)
        {
            _loggerFactory = factory;
        }

        public static ILogger GetLogger(object obj)
        {
            return GetLogger(obj.GetType());
        }

        public static ILogger GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        public static ILogger GetLogger(Type type)
        {
            return _loggerFactory.CreateLogger(type);
        }
    }
}