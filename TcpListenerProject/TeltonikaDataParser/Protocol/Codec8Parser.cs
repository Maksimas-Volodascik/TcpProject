using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TcpListenerProject.TeltonikaDataParser.Decoder;
using TcpListenerProject.TeltonikaDataParser.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TcpListenerProject.TeltonikaDataParser.Protocol
{
    public class Codec8Parser : IDecoder
    {
        public DataReader _parser;
        public Codec Codec => Codec.Codec8;
        

        public Dictionary<string, object?> Parse(PacketResult packet)
        {
            Dictionary<string, object?> record = new Dictionary<string, object?>();

            _parser = new DataReader(packet.Body);
    
            if (packet.Body.Length < 35) //minimum record 35 (without header)
                return null;

            long unixSeconds = BitConverter.ToInt64(_parser.ReadData(8)); //timestamp

            record.Add("Timestamp", DateTimeOffset
                .FromUnixTimeMilliseconds(unixSeconds)
                .UtcDateTime);

            record.Add("Codec", packet.Header.CodecID);
            record.Add("Priority", _parser.ReadData(1)[0]); // to dec
            record.Add("Longitude", BitConverter.ToInt32(_parser.ReadData(4)));
            record.Add("Latitude", BitConverter.ToInt32(_parser.ReadData(4)));
            record.Add("Altitude", BitConverter.ToInt16(_parser.ReadData(2)));
            record.Add("Angle", BitConverter.ToInt16(_parser.ReadData(2)));
            record.Add("Satellites", _parser.ReadData(1)[0]);
            record.Add("Speed", BitConverter.ToInt16(_parser.ReadData(2)));
            record.Add("EventIoId", _parser.ReadData(1)[0]);
            record.Add("TotalIDs", _parser.ReadData(1)[0]);
            
            if (Convert.ToInt32(record["TotalIDs"]) > 0) ParseIoElements(record);

            //var json = JsonSerializer.Serialize(record, new JsonSerializerOptions
            //{
            //    WriteIndented = true
            //});
            //Console.WriteLine(json);

            return record;
        }

        public void ParseIoElements(Dictionary<string, object?> record)
        {
            int count = _parser.ReadData(1)[0];
            for (int i = 0; i < count; i++)
            {
                var ioId = _parser.ReadData(1)[0];
                var value = _parser.ReadData(1)[0];

                record.Add(ioId.ToString(), value);
            }

            count = _parser.ReadData(1)[0];
            for (int i = 0; i < count; i++)
            {
                var ioId = _parser.ReadData(1)[0];
                var value = BitConverter.ToUInt16(_parser.ReadData(2));

                record.Add(ioId.ToString(), value);
            }

            count = _parser.ReadData(1)[0];
            for (int i = 0; i < count; i++)
            {
                var ioId = _parser.ReadData(1)[0];
                var value = BitConverter.ToUInt32(_parser.ReadData(4));

                record.Add(ioId.ToString(), value);
            }

            count = _parser.ReadData(1)[0];
            for (int i = 0; i < count; i++)
            {
                var ioId = _parser.ReadData(1)[0];
                var value = BitConverter.ToUInt64(_parser.ReadData(8));

                record.Add(ioId.ToString(), value);
            }
        }
    }
}
