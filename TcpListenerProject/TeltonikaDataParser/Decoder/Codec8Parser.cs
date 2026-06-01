using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpListenerProject.TeltonikaDataParser.Decoder
{
    public class Codec8Parser : IDecoder
    {
        public Codec Codec => Codec.Codec8;

        public JsonResult Parse(byte[] rawMessage)
        {
            return new JsonResult
            {
                Data = rawMessage
            };

        }
    }
}
