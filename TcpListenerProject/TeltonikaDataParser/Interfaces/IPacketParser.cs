using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpListenerProject.TeltonikaDataParser.Decoder;

namespace TcpListenerProject.TeltonikaDataParser.Interfaces
{
    public interface IPacketParser
    {
        PacketResult Parse(byte[] rawMessage);
    }
}
