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
            for(int i =0; i< PacketDict.Count; i++)
            {
                
            }
        }
    }
}
