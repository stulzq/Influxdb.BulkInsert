// #region File Annotation
// 
// Author：Zhiqiang Li
// 
// FileName：InfluxConnectionSetting.cs
// 
// Project：Influxdb.BulkInsert
// 
// CreateDate：2018/09/28
// 
// Note: The reference to this document code must not delete this note, and indicate the source!
// 
// #endregion

using System;

namespace Influxdb.BulkInsert
{
    public class InfluxConnectionSetting
    {
        public InfluxConnectionSetting()
        {
            
        }
        public InfluxConnectionSetting(string connectionString)
        {
            try
            {
                var temp = connectionString.Trim().Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in temp)
                {
                    if (item.StartsWith("server", true, null))
                    {
                        Server = item.Substring(7);
                    }
                    else if (item.StartsWith("port", true, null))
                    {
                        Port = int.Parse(item.Substring(5));
                    }
                    else if (item.StartsWith("username", true, null))
                    {
                        UserName = item.Substring(9);
                    }
                    else if (item.ToLower().StartsWith("password",true,null))
                    {
                        Password = item.Substring(9);
                    }
                    else if (item.StartsWith("database", true, null))
                    {
                        Database = item.Substring(9);
                    }
                    else if (item.StartsWith("bitchsize", true, null))
                    {
                        BitchSize = int.Parse(item.Substring(10));
                    }
                    else if (item.StartsWith("timeout", true, null))
                    {
                        Timeout = int.Parse(item.Substring(8));
                    }
                }
            }
            catch (Exception e)
            {
                throw new InfluxConnectionSyntaxException("There is a syntax error in the connection string.", e);
            }
            
        }
        public string Server { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public int BitchSize { get; set; } = 10;

        /// <summary>
        /// Unit second.
        /// </summary>
        public int Timeout { get; set; } = 30;
    }
}