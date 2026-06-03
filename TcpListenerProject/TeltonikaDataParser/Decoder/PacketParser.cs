using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpListenerProject.TeltonikaDataParser.Interfaces;
using TcpListenerProject.TeltonikaDataParser.Protocol;

namespace TcpListenerProject.TeltonikaDataParser.Decoder
{
    public class PacketParser : IPacketParser
    {
        public PacketResult Parse(byte[] rawMessage)
        {
            var recordSize = BitConverter.ToInt32(rawMessage, 4);
            var codecId = (Codec)rawMessage[8];
            var numberOfRecords = rawMessage[9];

            return new PacketResult
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
