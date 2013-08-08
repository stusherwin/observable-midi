using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SetupManager.Core
{
    public class SetupReceiver
    {
        private readonly IMidiDevice _midiDevice;
        private const int END_OF_MESSAGES_TIMEOUT = 1000;

        private static readonly SysExMessage REQUEST_SETUP_MESSAGE = new SysExMessage(new byte[]
        {
            0xF0, 0x41, 0x10, 0x00, 0x00, 0x2B, 0x11, 0x10, 0x00, 0x00, 0x00, 0x00, 0x07, 0x0F, 0x0B, 0x41, 0xF7
        });

        public SetupReceiver(IMidiDevice midiDevice)
        {
            _midiDevice = midiDevice;
        }

        public Setup RequestSetup()
        {
            var receivedMessages = new List<SysExMessage>();
            DateTime lastMessageReceived = DateTime.MaxValue;

            using(_midiDevice.StartReceiving(m =>
            {
                receivedMessages.Add(m);
                //Console.WriteLine("Sysex (name {0}): {1}", m.IsNameMessage? "Y" : "N", Encoding.ASCII.GetString(m.Bytes.Where(b => b > 31).ToArray()));
                //Console.WriteLine("\t{0}", String.Join(" ", m.Bytes.Select(b => b.ToString("X2"))));
                lastMessageReceived = DateTime.Now;
            }))
            {
                _midiDevice.Send(REQUEST_SETUP_MESSAGE);

                while ((DateTime.Now - lastMessageReceived).TotalMilliseconds < END_OF_MESSAGES_TIMEOUT)
                {
                    Thread.Sleep(10);
                }
                //Console.WriteLine("No more messages");

                return Setup.Load(receivedMessages);    
            }
        }
    }
}