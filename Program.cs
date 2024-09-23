// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.IO;
using JSONPacketDecode;
using Newtonsoft.Json;


namespace JsonDecode{
    class Program
    {
        static void Main()
        {

            DataDecoder decoderobj = new DataDecoder();

            List<Byte> FwByteData = new List<byte>();
            FwByteData.Add(0x25);
            FwByteData.Add(0x31);
            FwByteData.Add(0x42);

            byte header = FwByteData[0];

            #region json deserialization
            
            string filePath = "D:\\Apps\\JSONPacketDecode\\SampleJSON.json";  // Path to your JSON file

            string json = File.ReadAllText(filePath);  // Read the JSON file

            RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(json);
            #endregion

            foreach (var packet in rootObject.Packets)
            {
                string headertoHex = "0x" + header.ToString("X2");
                if(headertoHex == packet.Header)
                {
                    Dictionary<string, Dictionary<uint, uint[]>> pkt_field_Bitrange = decoderobj.DecoderMethod(packet);
                }
            }

            #region printing to console
            // Output the deserialized data
            Console.WriteLine($"SpecVersion: {rootObject.SpecVersion}");

            foreach (var packet in rootObject.Packets)
            {
                Console.WriteLine($"PacketName: {packet.PacketName}");
                Console.WriteLine($"Header: {packet.Header}");

                foreach (var field in packet.Fields)
                {
                    Console.WriteLine($"  FieldName: {field.FieldName}");
                    Console.WriteLine($"  BitRange: {field.BitRange}");
                    Console.WriteLine($"  Description: {field.Description}");
                    Console.WriteLine($"  Max: {field.Max}");
                    Console.WriteLine($"  Min: {field.Min}");
                }
            }
            #endregion
        }
    }
}


