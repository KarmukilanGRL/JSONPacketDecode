using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JSONPacketDecode
{
    public class DataDecoder
    {
        public Dictionary<string, Dictionary<uint, BitMsbLsb>> DecoderMethod(Packet packet)
        {
            Dictionary<string, Dictionary<uint, BitMsbLsb>> dict = new Dictionary<string, Dictionary<uint, BitMsbLsb>> { };
            foreach (var payload in packet.Fields)
            {
                Dictionary<uint, BitMsbLsb> field_bitrange = new Dictionary<uint, BitMsbLsb>();
                string bitrange = payload.BitRange;

                // Split the bitRange by '|'
                string[] BytesPair = bitrange.Split('|');

                foreach (string ByteSet in BytesPair)
                {
                    // Split each range by '-'
                    string[] parts = ByteSet.Split('-');

                    if (parts.Length == 2)
                    {
                        // Extract the key and value
                        uint key = uint.Parse(parts[0].Replace("B", "").Trim());
                        uint[] value = new uint[2];


                        string[] bitparts = parts[1].Split(':');
                        for (int i = 0; i < bitparts.Length; i++)
                        {
                            value[i] = uint.Parse(bitparts[i].Replace("b", "").Trim());
                        }

                        BitMsbLsb values = new BitMsbLsb(key, value[0], value[1]);

                        field_bitrange[key] = values;

                        // Add to dictionary
                        dict[payload.FieldName] = field_bitrange;
                    }
                }
            }
            return dict;
        }

        public void PacketDecode(Dictionary<string, Dictionary<uint, BitMsbLsb>> PacketDict)
        {
            for (int i = 0; i < PacketDict.Count; i++)
            {

            }
        }

        public byte GetByteFromBitrange(int bitrange)
        {
            // Ensure the bitrange does not exceed the maximum number of bits in a byte (8 bits)
            if (bitrange < 0 || bitrange > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(bitrange), "Bitrange must be between 0 and 8.");
            }

            // Left shift by bitrange and subtract 1 to get the corresponding value
            int value = (1 << bitrange) - 1;

            // Return the value as a hexadecimal string
            return (byte)value;
        }

        public void PayloadDecodeShifting(DataDecoder decoderobj, List<byte> FwByteData, Dictionary<uint, BitMsbLsb> bitInfoDict)
        {
            var bitInfo = bitInfoDict.Values.FirstOrDefault();

            uint bit_msb = bitInfo.bit_msb;
            uint bit_lsb = bitInfo.bit_lsb;
            uint curr_byte = bitInfo.bytevalue;

            // to calculate bitrange
            int bitrange = (int)((bit_msb - bit_lsb) + 1);

            // Create bitorder string
            string strbitorder = $"[{bit_msb}:{bit_lsb}]";
            byte idata = FwByteData[(int)curr_byte + 1];
            byte bytetoAnd = decoderobj.GetByteFromBitrange(bitrange);

            byte itemp = (byte)((idata >> (int)bit_lsb) & bytetoAnd);

            // Optionally do something with bitorder
            Console.WriteLine(strbitorder);  // Example usage

        }

        public void PayloadDecodeShiftingForMultipleBytes(DataDecoder decoderobj, List<byte> FwByteData, Dictionary<uint, BitMsbLsb> bitInfoDict)
        {
            StringBuilder strbitorder_new = new StringBuilder();
            List<byte> idata = new List<byte>();

            for (int i = 0; i < bitInfoDict.Values.Count; i++)
            {
                var bitInfo = bitInfoDict.Values.ElementAt(i);

                uint bit_msb = bitInfo.bit_msb;
                uint bit_lsb = bitInfo.bit_lsb;
                uint curr_byte = bitInfo.bytevalue;

                strbitorder_new.Append($"B{curr_byte}[{bit_msb}:{bit_lsb}]");
                if (i < bitInfoDict.Values.Count - 1)
                    strbitorder_new.Append($" + ");

                idata.Add(FwByteData[(int)curr_byte + 1]);
            }

            string strbitorder = strbitorder_new.ToString();
        }
    }
}
