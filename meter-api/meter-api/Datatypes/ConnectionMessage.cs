using System.Data;

namespace meter_api.Datatypes
{
    public class ConnectionMessage
    {
        public required string MeterId { get; set; }
        public required ConnectionState State { get; set; }
    }
}
