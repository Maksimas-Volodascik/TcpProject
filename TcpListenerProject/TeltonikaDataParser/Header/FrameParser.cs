using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpListenerProject.TeltonikaDataParser.Header
{
    public class FrameParser : IFrameParser
    {
        public FrameResult Parse(byte[] rawMessage)
        {
            var recordSize = BitConverter.ToInt32(rawMessage, 4);
            var codecId = (Codec)rawMessage[8];
            var numberOfRecords = rawMessage[9];

            return new FrameResult
            {
                Header = new ProtocolHeader
                {
                    CodecID = codecId,
                    RecordSize = recordSize,
                    NumberOfRecords = numberOfRecords
                },
                Body = rawMessage.Skip(10).ToArray()

            };
        }
    }
}
