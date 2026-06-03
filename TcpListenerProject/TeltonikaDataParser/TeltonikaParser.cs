using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpListenerProject.TeltonikaDataParser.Decoder;
using TcpListenerProject.TeltonikaDataParser.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TcpListenerProject.TeltonikaDataParser
{
    public class TeltonikaParser : ITeltonikaParser
    {
        private readonly IPacketParser _packetParser;
        private readonly IDecoderFactory _decoderFactory;
        public TeltonikaParser(IPacketParser packetParser, IDecoderFactory decoderFactory)
        {
            _packetParser = packetParser;
            _decoderFactory = decoderFactory;

        }
        public Elements Parse(byte[] rawMessage)
        {
            var packet = _packetParser.Parse(rawMessage);

            IDecoder decoder = _decoderFactory.Create(packet.Header.CodecID);

            return decoder.Parse(packet);
        }
    }
}
