using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpListenerProject.TeltonikaDataParser.Header;

namespace TcpListenerProject.TeltonikaDataParser.Decoder
{
    public class Codec8EParser : IDecoder
    {
        public Codec Codec => Codec.Codec8E;

        public AvlRecord Parse(PacketResult packet)
        {
            throw new NotImplementedException();
        }
    }
}
