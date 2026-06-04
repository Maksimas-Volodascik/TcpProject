using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TcpListenerProject.TeltonikaDataParser.Decoder;
using TcpListenerProject.TeltonikaDataParser.Interfaces;

namespace TcpListenerProject.TeltonikaDataParser.Protocol
{
    public class Codec8EParser : IDecoder
    {
        public DataReader _parser;
        public Codec Codec => Codec.Codec8E;

        public Elements Parse(PacketResult packet)
        {
            _parser = new DataReader(packet.Body);
            GpsElement GpsElements = new GpsElement();

            if (packet.Body.Length < 35) //minimum record 35 (without header)
                return null;

            long unixSeconds = BitConverter.ToInt64(_parser.ReadData(8)); //timestamp

            GpsElements.Timestamp = DateTimeOffset
                .FromUnixTimeMilliseconds(unixSeconds)
                .UtcDateTime;

            GpsElements.Priority = _parser.ReadData(1)[0]; ; // to dec
            GpsElements.Longitude = BitConverter.ToInt32(_parser.ReadData(4));
            GpsElements.Latitude = BitConverter.ToInt32(_parser.ReadData(4));
            GpsElements.Altitude = BitConverter.ToInt16(_parser.ReadData(2));
            GpsElements.Angle = BitConverter.ToInt16(_parser.ReadData(2));
            GpsElements.Satellites = _parser.ReadData(1)[0];
            GpsElements.Speed = BitConverter.ToInt16(_parser.ReadData(2));
            GpsElements.EventIoId = BitConverter.ToUInt16(_parser.ReadData(2));
            GpsElements.TotalIDs = BitConverter.ToUInt16(_parser.ReadData(2));


            IoElement ioElement = new IoElement();
            if (GpsElements.TotalIDs > 0)
            {
                ioElement = ParseIoElements();
            }

            var rec = new Elements
            {
                Header = packet.Header,
                GpsElements = GpsElements,
                IoElements = ioElement
            };

            var json = JsonSerializer.Serialize(rec);

            Console.WriteLine(json);
            return rec;
        }

        public IoElement ParseIoElements()
        {
            IoElement ioElement = new IoElement();
            ioElement.N1.Count = BitConverter.ToUInt16(_parser.ReadData(2));

            for (int i = 0; i < ioElement.N1.Count; i++)
            {
                ioElement.N1.Items.Add(new IoPair<byte>
                {
                    IoId = BitConverter.ToUInt16(_parser.ReadData(2)),
                    Value = _parser.ReadData(1)[0]
                });
            }

            ioElement.N2.Count = BitConverter.ToUInt16(_parser.ReadData(2));
            Console.WriteLine("N2 Count {0}", ioElement.N2.Count);

            for (int i = 0; i < ioElement.N2.Count; i++)
            {

                ioElement.N2.Items.Add(new IoPair<ushort>
                {
                    IoId = BitConverter.ToUInt16(_parser.ReadData(2)),
                    Value = BitConverter.ToUInt16(_parser.ReadData(2))
                });
            }

            ioElement.N4.Count = BitConverter.ToUInt16(_parser.ReadData(2));
            for (int i = 0; i < ioElement.N4.Count; i++)
            {
                ioElement.N4.Items.Add(new IoPair<uint>
                {
                    IoId = BitConverter.ToUInt16(_parser.ReadData(2)),
                    Value = BitConverter.ToUInt32(_parser.ReadData(4))
                });
            }

            ioElement.N8.Count = BitConverter.ToUInt16(_parser.ReadData(2));
            for (int i = 0; i < ioElement.N8.Count; i++)
            {
                ioElement.N8.Items.Add(new IoPair<ulong>
                {
                    IoId = BitConverter.ToUInt16(_parser.ReadData(2)),
                    Value = BitConverter.ToUInt64(_parser.ReadData(8))
                });
            }

            ioElement.NX.Count = BitConverter.ToUInt16(_parser.ReadData(2));
            for (int i = 0; i < ioElement.NX.Count; i++)
            {
                ushort ioId = BitConverter.ToUInt16(_parser.ReadData(2));
                ushort length = BitConverter.ToUInt16(_parser.ReadData(2));
                ioElement.NX.Items.Add(new IoPair<int?>
                {
                    IoId = ioId,
                    Value = (int)BitConverter.ToUInt32(_parser.ReadData(length))
                });
            }


            return ioElement;
        }
    }
}
