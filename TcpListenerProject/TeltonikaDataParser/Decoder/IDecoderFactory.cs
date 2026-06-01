using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpListenerProject.TeltonikaDataParser.Decoder
{
    public interface IDecoderFactory
    {
        public IDecoder Create(Codec codecId);
    }
}
