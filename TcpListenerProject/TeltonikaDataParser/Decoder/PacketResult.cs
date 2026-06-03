using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpListenerProject.TeltonikaDataParser.Protocol;

namespace TcpListenerProject.TeltonikaDataParser.Decoder
{
    public class PacketResult
    {
        public ProtocolHeader Header { get; set; }
        public byte[] Body { get; set; }
    }

    public class ProtocolHeader
    {
        public int RecordSize { get; set; }
        public Codec CodecID { get; set; }
        public int NumberOfRecords { get; set; }
    }
}
