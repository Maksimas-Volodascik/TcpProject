using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpListenerProject.TeltonikaDataParser.Decoder
{
    public interface IDecoder
    {
        public Codec Codec { get; }
        public JsonResult Parse(byte[] rawMessage);
    }
}
