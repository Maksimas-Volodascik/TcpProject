using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpListenerProject.TeltonikaDataParser.Protocol;

namespace TcpListenerProject.TeltonikaDataParser.Interfaces
{
    public interface IDecoderFactory
    {
        public IDecoder Create(Codec codecId);
    }
}
