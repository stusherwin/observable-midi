using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Sanford.Multimedia.Midi;
using System.IO;

namespace SysexTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using(InputDevice rdIn = new InputDevice(0))
            using(OutputDevice rdOut = new OutputDevice(1))
            {
                //for (int i = 0; i < InputDevice.DeviceCount; i++)
                //{
                //    Console.WriteLine("{0}: {1}", i, InputDevice.GetDeviceCapabilities(i).name);
                //}
                //for (int i = 0; i < OutputDevice.DeviceCount; i++)
                //{
                //    Console.WriteLine("{0}: {1}", i, OutputDevice.GetDeviceCapabilities(i).name);
                //}
                //Console.ReadKey();

                List<byte[]> receivedMessages = new List<byte[]>();
                //List<byte[]> dataParts = new List<byte[]>();
                //List<byte> checksums = new List<byte>();
                int msgNo = 0;
                rdIn.SysExMessageReceived += (_, e) =>
                                                 {
                                                     Console.WriteLine("SysEx received: {0}", msgNo++);
                                                     //Console.WriteLine("Length {0}, Message type {1}, Status {2}, SysEx type {3}", e.Message.Length, e.Message.MessageType, e.Message.Status, e.Message.SysExType);

                                                     //foreach (byte b in e.Message.GetBytes())
                                                     //    Console.Write("{0:x} ");
                                                     //Console.WriteLine();
                                                     //Console.WriteLine();
                                                     var bytes = e.Message.GetBytes();
                                                     receivedMessages.Add(bytes);
                                                 };
                rdIn.StartRecording();

                Console.WriteLine("Sending SysEx");
                //f0 41 10 00 00 2b 11 10 00 00 00 00 07 0f 0b 41 f7 
                rdOut.Send(
                    new SysExMessage(new byte[]
                                         {
                                             0xF0, 0x41, 0x10, 0x00, 0x00, 0x2B, 0x11, 0x10, 0x00, 0x00, 0x00, 0x00, 0x07,
                                             0x0F, 0x0B, 0x41, 0xF7
                                         }));

                //            Console.ReadKey();
                //            Console.WriteLine("Choose destination setup");
                //            Console.ReadKey();
                ////            foreach (var msg in receivedMessages)
                ////            {
                //                rdOut.Send(new SysExMessage(receivedMessages[1]));
                ////            }

                //Console.ReadKey();


                //byte[] header = new byte[] { 0xf0, 0x41, 0x10, 0x0, 0x0, 0x2b, 0x12 };
                //byte[] address = new byte[] { 0x10, 0x0, 0x0, 0x0 };
                //int messageLength = dataParts.Sum(p => p.Length) + header.Length + 2;
                //byte end = 0xf7;
                //byte[] checksumBytes = address.Concat(dataParts.SelectMany(p => p)).ToArray();
                //byte checksum = CalculateChecksum(checksumBytes);

                Console.ReadKey();

                //Console.WriteLine("Choose destination setup");

                //Console.ReadKey();

                receivedMessages = receivedMessages.OrderBy(m => (m[7] << 24) + (m[8] << 16) + (m[9] << 8) + m[10]).ToList();

                foreach (var msg in receivedMessages)
                {
                    //var dataPart = new byte[msg.Length - 13];
                    //for (int i = 11; i < msg.Length - 2; i++)
                    //{
                    //    dataPart[i - 11] = msg[i];
                    //}

                    //var chksm = new byte[msg.Length - 9];
                    //for (int i = 7; i < msg.Length - 2; i++)
                    //{
                    //    chksm[i - 7] = msg[i];
                    //}
                    //checksums.Add(CalculateChecksum(chksm));

                    //dataParts.Add(dataPart);
                }

                //byte[] header = new byte[] { 0xf0, 0x41, 0x10, 0x0, 0x0, 0x2b, 0x12 };
                //byte[] address = new byte[] { 0x10, 0x0, 0x0, 0x0 };
                //byte[] data = dataParts.SelectMany(p => p).ToArray();
                //byte[] checksumBytes = address.Concat(data).ToArray();
                //byte checksum = CalculateChecksum(checksumBytes);
                //byte end = 0xf7;
                //byte[] message = header.Concat(checksumBytes).Concat(new byte[] { checksum, end }).ToArray();

                //Console.WriteLine("Press a key to send it back");
                //Console.ReadKey();
                //Console.WriteLine("Sending sysex...");
                //DateTime start = DateTime.Now;
                //rdOut.Send(new SysExMessage(message));
                //Console.WriteLine("Done in " + (DateTime.Now - start).TotalMilliseconds + " ms");

                Console.WriteLine("Press a key to send it back (ESC to quit)");
                while (Console.ReadKey().Key != ConsoleKey.Escape)
                {
                    Console.WriteLine("Sending sysex...");
                    DateTime start = DateTime.Now;
                    foreach (var msg in receivedMessages)
                    {
                        rdOut.Send(new SysExMessage(msg));
                        Thread.Sleep(40);
                    }
                    Console.WriteLine("Done in " + (DateTime.Now - start).TotalMilliseconds + " ms");
                }

                rdIn.Close();
                rdOut.Close();

                using (var fs = File.CreateText("sysex.txt"))
                {
                    for (int i = 0; i < receivedMessages.Count; i++)
                    {
                        foreach (var b in receivedMessages[i])
                            fs.Write(b.ToString("x2") + " ");
                        fs.WriteLine();

                        foreach (var b in receivedMessages[i])
                            fs.Write(b >= 32 && b <= 126 ? " " + (char)b + " " : "-- ");
                        fs.WriteLine();

                        //foreach (var b in dataParts[i])
                        //    fs.Write(b.ToString("x") + " ");

                        //fs.WriteLine();

                        //fs.WriteLine(checksums[i].ToString("x"));
                        //fs.WriteLine();
                        fs.WriteLine();
                    }

                    //foreach (var b in message)
                    //    fs.Write(b.ToString("x") + " ");
                }
            }
        }

        static byte CalculateChecksum(byte[] bytes)
        {
            return (byte)(128 - (bytes.Sum(b => b) % 128));
        }
    }
}

