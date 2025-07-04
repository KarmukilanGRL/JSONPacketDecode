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

            

            #region json deserialization
            
            string filePath = "C:\\Users\\grlsw\\Downloads\\JSONPacketDecode\\SampleJSON.json";  // Path to your JSON file

            string json = File.ReadAllText(filePath);  // Read the JSON file

            RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(json);
            #endregion

            foreach (var packet in rootObject.Packets)
            {
                byte header = FwByteData[0];
                string headertoHex = "0x" + header.ToString("X2");
                if(headertoHex == packet.Header)
                {
                    List<string> DisplayPayloadvalues = new();
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
                            
                            // Find the corresponding field in the packet
                            Field? currentField = packet.Fields.FirstOrDefault(f => f.FieldName == payload_name);
                            
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
                            
                            // Check if the field has Values and get description/display
                            string valueDescription = "";
                            string valueDisplay = "";
                            if (currentField != null)
                            {
                                (valueDescription, valueDisplay) = decoderobj.GetValueInfo(currentField, itemp);
                            }
                            
                            // Add to DisplayPayloadvalues based on Displayable flag
                            if (currentField != null && currentField.Displayable)
                            {
                                if (!string.IsNullOrEmpty(valueDisplay))
                                {
                                    DisplayPayloadvalues.Add($"{payload_name} : {valueDisplay}");
                                }
                                else
                                {
                                    DisplayPayloadvalues.Add($"{payload_name} : {itemp}");
                                }
                            }
                            
                            Console.WriteLine($"    Payload Name : {payload_name}");
                            Console.WriteLine($"    Bit Order : {strbitorder}");
                            Console.WriteLine($"    Decoded Value : {itemp}");
                            
                            // Display additional info if values are found
                            if (!string.IsNullOrEmpty(valueDescription))
                            {
                                Console.WriteLine($"    Description : {valueDescription}");
                                Console.WriteLine($"    Display : {valueDisplay}");
                            }
                            Console.WriteLine($"    ----------------------------------");
                        }
                        Console.WriteLine($"    Display String : {packet.PacketName}[{packet.Header}] - {{ {string.Join(" | ", DisplayPayloadvalues)} }}");
                        Console.WriteLine($"    ----------------------------------");
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


