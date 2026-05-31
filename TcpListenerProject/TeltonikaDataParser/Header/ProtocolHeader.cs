using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpListenerProject.TeltonikaDataParser.Header
{
    public class ProtocolHeader
    {
        public int RecordSize { get; set; }
        public Codec CodecID { get; set; }
        public int NumberOfRecords { get; set; }

    }
}
