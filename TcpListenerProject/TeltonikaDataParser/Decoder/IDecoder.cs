using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpListenerProject.TeltonikaDataParser.Header;

namespace TcpListenerProject.TeltonikaDataParser.Decoder
{
    public interface IDecoder
    {
        public Codec Codec { get; }
        public AvlRecord Parse(PacketResult packet);
    }
}
