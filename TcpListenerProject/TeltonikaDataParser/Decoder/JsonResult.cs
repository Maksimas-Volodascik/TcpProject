using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpListenerProject.TeltonikaDataParser.Header;

namespace TcpListenerProject.TeltonikaDataParser.Decoder
{
    public class JsonResult
    {
        //public HeaderResult Header { get; set; }
        public byte[] Data { get; set; }
        /*
        public DateTime Timestamp { get; set; }
        public int Priority { get; set; }
        public int Longitude { get; set; }
        public int Latitude { get; set; }
        public int Altitude { get; set; }
        public int Angle { get; set; }
        public int Satellites { get; set; }
        public int Speed { get; set; }

        public Dictionary<int, byte> N1 { get; set; }
        public Dictionary<int, ushort> N2 { get; set; }
        public Dictionary<int, uint> N4 { get; set; }
        public Dictionary<int, ulong> N8 { get; set; }*/
    }
}
