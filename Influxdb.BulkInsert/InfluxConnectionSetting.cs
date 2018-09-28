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

namespace Influxdb.BulkInsert
{
    public class InfluxConnectionSetting
    {
        public string Server { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
    }
}