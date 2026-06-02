using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TcpListenerProject.TeltonikaDataParser.Header;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TcpListenerProject.TeltonikaDataParser.Decoder
{
    public class Codec8Parser : IDecoder
    {
        public DataReader _parser;
        public Codec Codec => Codec.Codec8;

        public AvlRecord Parse(PacketResult packet)
        {
            _parser = new DataReader(packet.Body);
            GpsElement GpsElements = new GpsElement();
    
            if (packet.Body.Length < 35) //minimum record 35 (without header)
                return null;

            var currentBytes = _parser.ReadData(8); //timestamp

            //Console.WriteLine(Int32.Parse(currentBytes));
            long unixSeconds = BinaryPrimitives.ReadInt64BigEndian(currentBytes);

            GpsElements.Timestamp = DateTimeOffset
                .FromUnixTimeSeconds(unixSeconds)
                .UtcDateTime;

            GpsElements.Priority = _parser.ReadData(1)[0]; ; // to dec
            GpsElements.Longitude = BitConverter.ToInt32(_parser.ReadData(4));
            GpsElements.Latitude = BitConverter.ToInt32(_parser.ReadData(4));
            GpsElements.Altitude = BitConverter.ToInt16(_parser.ReadData(2));
            GpsElements.Angle = BitConverter.ToInt16(_parser.ReadData(2));
            GpsElements.Satellites = _parser.ReadData(1)[0];
            GpsElements.Speed = BitConverter.ToInt16(_parser.ReadData(2));
            GpsElements.EventIoId = _parser.ReadData(1)[0];
            GpsElements.TotalIDs = _parser.ReadData(1)[0];


            IoElement ioElement = new IoElement();
            if (GpsElements.TotalIDs > 0)
            {
                ioElement = ParseIoElements();
            }

            var rec = new AvlRecord
            {
                Header = packet.Header,
                GpsElements = GpsElements,
                IoElements = ioElement
            };

            string json = JsonSerializer.Serialize(rec, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            Console.WriteLine(json);
            return rec;
        }

        public IoElement ParseIoElements()
        {
            IoElement ioElement = new IoElement();

            ioElement.N1.Count = BitConverter.ToInt16(_parser.ReadData(1));
            for (int i = 0; i < ioElement.N1.Count; i++)
            {
                ioElement.N1.Items.Add(new IoPair<byte>
                {
                    IoId = BitConverter.ToUInt16(_parser.ReadData(1)),
                    Value = _parser.ReadData(1)[0]
                });
            }

            ioElement.N2.Count = BitConverter.ToInt16(_parser.ReadData(1));
            for (int i = 0; i < ioElement.N2.Count; i++)
            {
                ioElement.N2.Items.Add(new IoPair<ushort>
                {
                    IoId = BitConverter.ToUInt16(_parser.ReadData(1)),
                    Value = BitConverter.ToUInt16(_parser.ReadData(2))
                });
            }

            ioElement.N4.Count = BitConverter.ToInt16(_parser.ReadData(1));
            for (int i = 0; i < ioElement.N4.Count; i++)
            {
                ioElement.N4.Items.Add(new IoPair<uint>
                {
                    IoId = BitConverter.ToUInt16(_parser.ReadData(1)),
                    Value = BitConverter.ToUInt32(_parser.ReadData(4))
                });
            }

            ioElement.N8.Count = BitConverter.ToInt16(_parser.ReadData(1));
            for (int i = 0; i < ioElement.N8.Count; i++)
            {
                ioElement.N8.Items.Add(new IoPair<ulong>
                {
                    IoId = BitConverter.ToUInt16(_parser.ReadData(1)),
                    Value = BitConverter.ToUInt64(_parser.ReadData(8))
                });
            }
            return ioElement;
        }
    }
}
