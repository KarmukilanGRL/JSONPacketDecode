using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONPacketDecode
{
    public class PacketDecode
    {
        public string PacketName = "";
        public string Header = "";

        public string FieldName = "";
        public string BitRange = "";
        public string Description = "";
        public string Max = "";
        public string Min = "";

        public string[] Fields = [] ;
    }

    public class RootObject
    {
        public string SpecVersion { get; set; }
        public List<Packet> Packets { get; set; }
    }

    public class Packet
    {
        public string PacketName { get; set; }
        public string Header { get; set; }
        public List<Field> Fields { get; set; }
    }

    public class Field
    {
        public string FieldName { get; set; }
        public string BitRange { get; set; }
        public string Description { get; set; }
        public int Max { get; set; }
        public int Min { get; set; }
    }

    public class BitMsbLsb
    {
        public uint bytevalue { get; set; }
        public uint bit_lsb { get; set; }
        public uint bit_msb { get; set; }
        public uint[] bit_lsbmsb { get; set; }

        public BitMsbLsb(uint by, uint bitlsb, uint bitmsb)
        {
            bytevalue = by;
            bit_lsb = bitlsb;
            bit_msb = bitmsb;
            bit_lsbmsb = [bitlsb, bitmsb];
        }
    }
}
