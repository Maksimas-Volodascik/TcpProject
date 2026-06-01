using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpListenerProject.TeltonikaDataParser.Decoder;
using TcpListenerProject.TeltonikaDataParser.Header;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TcpListenerProject.TeltonikaDataParser
{
    public class TeltonikaParser : ITeltonikaParser
    {
        private readonly IHeaderParser _headerParser;
        private readonly IDecoderFactory _decoderFactory;
        public TeltonikaParser(IHeaderParser headerParser, IDecoderFactory decoderFactory)
        {
            _headerParser = headerParser;
            _decoderFactory = decoderFactory;

        }
        public JsonResult Parse(byte[] rawMessage)
        {
            var header = _headerParser.Parse(rawMessage);

            IDecoder decoder = _decoderFactory.Create(header.Header.CodecID);

            return decoder.Parse(rawMessage);
        }
    }
}
