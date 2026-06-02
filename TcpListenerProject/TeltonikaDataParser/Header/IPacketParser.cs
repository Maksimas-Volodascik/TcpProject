using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpListenerProject.TeltonikaDataParser.Header
{
    public interface IPacketParser
    {
        PacketResult Parse(byte[] rawMessage);
    }
}
