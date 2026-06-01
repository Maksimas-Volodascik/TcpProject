using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpListenerProject.TeltonikaDataParser.Decoder;

namespace TcpListenerProject.TeltonikaDataParser
{
    public interface ITeltonikaParser
    {
        public JsonResult Parse(byte[] rawMessage);
    }
}
