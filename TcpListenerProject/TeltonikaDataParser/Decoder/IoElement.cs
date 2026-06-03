using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpListenerProject.TeltonikaDataParser.Decoder
{
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
