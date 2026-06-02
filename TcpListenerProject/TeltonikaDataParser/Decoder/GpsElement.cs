using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpListenerProject.TeltonikaDataParser.Decoder
{
    public class GpsElement
    {
        public DateTimeOffset Timestamp { get; set; }
        public int Priority { get; set; }
        public int Longitude { get; set; }
        public int Latitude { get; set; }
        public int Altitude { get; set; }
        public int Angle { get; set; }
        public int Satellites { get; set; }
        public int Speed { get; set; }
        public int EventIoId { get; set; }
        public int TotalIDs { get; set; }
    }
}
