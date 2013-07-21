using System;
using System.Collections.Generic;
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
                lastMessageReceived = DateTime.Now;
            }))
            {
                _midiDevice.Send(REQUEST_SETUP_MESSAGE);

                while ((DateTime.Now - lastMessageReceived).TotalMilliseconds < END_OF_MESSAGES_TIMEOUT)
                {
                    Thread.Sleep(10);
                }

                return Setup.Load(receivedMessages);    
            }
        }
    }
}