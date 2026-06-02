using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpListenerProject.TeltonikaDataParser.Decoder
{
    public class DataReader
    {
        private readonly byte[] _data;
        private int _offset;
        public DataReader(byte[] data)
        {
            _data = data;
            _offset = 0;
        }

        public byte[] ReadData (int size)
        {
            var result = new byte[size];

            Array.Copy(_data, _offset, result, 0, size);

            _offset += size;

            return result;
        }
    }
}
