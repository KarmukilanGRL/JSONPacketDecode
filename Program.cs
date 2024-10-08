﻿// See https://aka.ms/new-console-template for more information

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

            

            #region json deserialization
            
            string filePath = "D:\\Apps\\JSONPacketDecode\\JSONPacketDecode\\SampleJSON.json";  // Path to your JSON file

            string json = File.ReadAllText(filePath);  // Read the JSON file

            RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(json);
            #endregion

            foreach (var packet in rootObject.Packets)
            {
                byte header = FwByteData[0];
                string headertoHex = "0x" + header.ToString("X2");
                if(headertoHex == packet.Header)
                {
                    Dictionary<string, Dictionary<uint, BitMsbLsb>> pkt_field_Bitrange = decoderobj.DecoderMethod(packet);
                    if (FwByteData.Count > 1)
                    {
                        Console.WriteLine($"Packet Name : {packet.PacketName}");

                        foreach (var kvp in pkt_field_Bitrange)  // Iterate over the outer dictionary
                        {
                            string strbitorder = "";
                            byte itemp = 0;
                            string payload_name = kvp.Key;
                            Dictionary<uint, BitMsbLsb> bitInfoDict = kvp.Value;
                            if(bitInfoDict.Count == 1)
                            { 
                                (strbitorder, itemp) = decoderobj.PayloadDecodeShifting(FwByteData, bitInfoDict); // to return idata and strbitoder
                            }
                            else
                            {
                                uint itempdata = 0;
                                (strbitorder, itempdata) = decoderobj.PayloadDecodeShiftingForMultipleBytes(FwByteData, bitInfoDict);
                                itemp = (byte)itempdata;
                            }

                            Console.WriteLine($"    Payload Name : {payload_name}");
                            Console.WriteLine($"    Bit Order : {strbitorder}");
                            Console.WriteLine($"    Decoded Value : {itemp}");
                            Console.WriteLine($"    ----------------------------------");
                        }

                        
                    }
                }
            }

            //#region printing to console
            //// Output the deserialized data
            //Console.WriteLine($"SpecVersion: {rootObject.SpecVersion}");

            //foreach (var packet in rootObject.Packets)
            //{
            //    Console.WriteLine($"PacketName: {packet.PacketName}");
            //    Console.WriteLine($"Header: {packet.Header}");

            //    foreach (var field in packet.Fields)
            //    {
            //        Console.WriteLine($"  FieldName: {field.FieldName}");
            //        Console.WriteLine($"  BitRange: {field.BitRange}");
            //        Console.WriteLine($"  Description: {field.Description}");
            //        Console.WriteLine($"  Max: {field.Max}");
            //        Console.WriteLine($"  Min: {field.Min}");
            //    }
            //}
            //#endregion
        }

        
    }
}


