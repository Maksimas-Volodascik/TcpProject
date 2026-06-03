using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpListenerProject.TeltonikaDataParser.Decoder
{
    public class Elements
    {
        public ProtocolHeader Header { get; set; }
        public GpsElement GpsElements { get; set; }
        public IoElement IoElements { get; set; }
    }
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
    public class IoElement
    {
        public IoGroup<byte> N1 { get; set; } = new();
        public IoGroup<ushort> N2 { get; set; } = new();
        public IoGroup<uint> N4 { get; set; } = new();
        public IoGroup<ulong> N8 { get; set; } = new();
    }

    public class IoGroup<T>
    {
        public int Count { get; set; }
        public List<IoPair<T>> Items { get; set; } = new();
    }

    public class IoPair<T>
    {
        public ushort IoId { get; set; }
        public T Value { get; set; }
    }
}
