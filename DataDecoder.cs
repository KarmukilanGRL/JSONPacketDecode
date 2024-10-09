using System;
using System.Collections;
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

        public (string, byte) PayloadDecodeShifting(List<byte> FwByteData, Dictionary<uint, BitMsbLsb> bitInfoDict)
        {
            string strbitorder = "";
            byte itemp = 0;
            try
            {
                var bitInfo = bitInfoDict.Values.FirstOrDefault();

                uint bit_msb = bitInfo.bit_msb;
                uint bit_lsb = bitInfo.bit_lsb;
                uint curr_byte = bitInfo.bytevalue;

                // to calculate bitrange
                int bitrange = (int)((bit_msb - bit_lsb) + 1);

                // Create bitorder string
                strbitorder = $"[{bit_msb}:{bit_lsb}]";
                byte idata = FwByteData[(int)curr_byte + 1];
                byte bytetoAnd = GetByteFromBitrange(bitrange);

                itemp = (byte)((idata >> (int)bit_lsb) & bytetoAnd);
            }
            catch (Exception ex)
            {

            }
            return (strbitorder, itemp);
        }

        public (string, uint) PayloadDecodeShiftingForMultipleBytes(List<byte> FwByteData, Dictionary<uint, BitMsbLsb> bitInfoDict)
        {
            StringBuilder strbitorder_new = new StringBuilder();
            List<byte> idata = new List<byte>();
            List<int> shiftValue = new List<int>();

            uint result = 0;
            int accumulatedShift = 0;
            byte idata_temp = 0;

            for (int i = 0; i < bitInfoDict.Values.Count; i++)
            {
                var bitInfo = bitInfoDict.Values.ElementAt(i);

                uint bit_msb = bitInfo.bit_msb;
                uint bit_lsb = bitInfo.bit_lsb;
                uint curr_byte = bitInfo.bytevalue;

                strbitorder_new.Append($"B{curr_byte}[{bit_msb}:{bit_lsb}]");
                if (i < bitInfoDict.Values.Count - 1)
                    strbitorder_new.Append($" + ");

                shiftValue.Add((int)(bit_msb - bit_lsb) + 1);

                

                //(_, idata_temp) = PayloadDecodeShifting(FwByteData, ); // to return idata and strbitoder
                // this fwbytedata shoudl be shifted before adding to idata list

                idata.Add(FwByteData[(int)curr_byte + 1]);
            }      

            // We iterate backwards because the first byte has the largest shift
            for (int i = idata.Count - 1; i >= 0; i--)
            {
                result |= (uint)(idata[i] << accumulatedShift); // Shift the current byte by the accumulated shift
                accumulatedShift += shiftValue[i]; // Accumulate the shift for the next byte (if any)
            }

            string strbitorder = strbitorder_new.ToString();
            uint itemp = result;

            return (strbitorder, itemp);
        }
    }
}
