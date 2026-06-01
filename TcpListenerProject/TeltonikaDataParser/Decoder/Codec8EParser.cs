using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpListenerProject.TeltonikaDataParser.Decoder
{
    public class Codec8EParser : IDecoder
    {
        public Codec Codec => Codec.Codec8E;

        public JsonResult Parse(byte[] rawMessage)
        {
            throw new NotImplementedException();
        }
    }
}
