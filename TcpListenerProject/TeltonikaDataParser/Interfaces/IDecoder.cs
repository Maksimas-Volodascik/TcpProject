using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpListenerProject.TeltonikaDataParser.Decoder;
using TcpListenerProject.TeltonikaDataParser.Protocol;

namespace TcpListenerProject.TeltonikaDataParser.Interfaces
{
    public interface IDecoder
    {
        public Codec Codec { get; }
        public Dictionary<string, object?> Parse(PacketResult packet);
    }
}
