using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpListenerProject.TeltonikaDataParser
{
    public interface ITeltonikaParser
    {
        public AvlRecord Parse(byte[] rawMessage);
    }
}
