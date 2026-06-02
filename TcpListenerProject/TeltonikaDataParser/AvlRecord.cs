using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpListenerProject.TeltonikaDataParser.Decoder;
using TcpListenerProject.TeltonikaDataParser.Header;

namespace TcpListenerProject.TeltonikaDataParser
{
    public class AvlRecord
    {
        public ProtocolHeader Header { get; set; }
        public GpsElement GpsElements { get; set; }
        public IoElement IoElements { get; set; }

    }
}
