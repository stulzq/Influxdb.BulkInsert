using System.Text;
using System.Threading.Tasks;

namespace Influxdb.BulkInsert
{
    public interface IInfluxBulkInsert
    {
        int BitchSize { get; }

        Task SendAsync(string data);

        Task SendAsync(StringBuilder sb);
    }
}