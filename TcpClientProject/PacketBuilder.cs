using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpClientProject
{
    public class PacketBuilder
    {
        public DateTimeOffset dateTime { get; set; }
        public PacketBuilder(DateTimeOffset _dateTime)
        {
            dateTime = _dateTime;
        }

        public string GetCodecString(double longitude, double latitude)
        {
            string longHex = DMSParser(longitude);
            string latHex = DMSParser(latitude);

            dateTime = dateTime.AddSeconds(1);
            string newDate = dateTime.ToUnixTimeMilliseconds().ToString("X16");

            return ($"000000000000004A8E01{newDate}01{longHex}{latHex}0000000000000000010005000100010100010011001D00010010015E2C880002000B000000003544C87A000E000000001DD7E06A00000100002994");
        }

        private string DMSParser(double coordValue)
        {
            //value = ( d + m/60 + s/3600 + ms/3600000 ) × p            
            int p = 10000000; //precision

            int d = (int)Math.Truncate(coordValue); //degrees
            double afterD = (coordValue - d) * 60;

            int m = (int)Math.Truncate(afterD); //minutes
            double afterM = (afterD - m) * 60;

            int s = (int)Math.Truncate(afterM); //seconds
            double afterS = (afterM - s) * 1000;

            int ms = (int)Math.Round(afterS, MidpointRounding.AwayFromZero); //milliseconds

            var value = (int)Math.Round((d + m / 60.0 + s / 3600.0 + ms / 3600000.0) * p);

            byte[] bytes = BitConverter.GetBytes(value); 
            if (BitConverter.IsLittleEndian) // latitude/longitude is big-endian. Flip value values
                Array.Reverse(bytes);

            return Convert.ToHexString(bytes);
        }
    }
}
