using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpListenerProject.TeltonikaDataParser.Decoder;
using TcpListenerProject.TeltonikaDataParser.Interfaces;

namespace TcpListenerProject.TeltonikaDataParser.Protocol
{
    public class Codec8EParser : IDecoder
    {
        public Codec Codec => Codec.Codec8E;

        public Elements Parse(PacketResult packet)
        {
            throw new NotImplementedException();
        }
    }
}
