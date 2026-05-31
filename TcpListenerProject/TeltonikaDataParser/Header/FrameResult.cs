using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpListenerProject.TeltonikaDataParser.Header
{
    public class FrameResult
    {
        public ProtocolHeader Header { get; set; }
        public byte[] Body { get; set; }
    }
}
