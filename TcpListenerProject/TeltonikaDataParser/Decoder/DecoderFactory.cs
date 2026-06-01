using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpListenerProject.TeltonikaDataParser.Decoder
{
    public class DecoderFactory : IDecoderFactory
    {
        private readonly Dictionary<Codec, IDecoder> _decoders;
        public DecoderFactory(IEnumerable<IDecoder> decoders)
        {
            _decoders = decoders.ToDictionary(x => x.Codec);
        }
        public IDecoder Create(Codec codecId)
        {
            if (_decoders.TryGetValue(codecId, out var decoder))
                return decoder;

            throw new ArgumentException($"Invalid codec protocol: {codecId}");
        }
    }
}
