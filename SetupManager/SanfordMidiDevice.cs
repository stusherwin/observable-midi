using System;
using Sanford.Multimedia.Midi;
using SetupManager.Core;
using San = Sanford.Multimedia.Midi;
using SysExMessage = SetupManager.Core.SysExMessage;

namespace SetupManager
{
    public class SanfordMidiDevice : IMidiDevice
    {
        private readonly San.InputDevice _input;
        private readonly San.OutputDevice _output;

        public SanfordMidiDevice(San.InputDevice inputDevice, San.OutputDevice outputDevice)
        {
            _input = inputDevice;
            _output = outputDevice;
        }

        public void Dispose()
        {
            _input.Close();
            _output.Close();
            _input.Dispose();
            _output.Dispose();
        }

        public void Send(SysExMessage message)
        {
            _output.Send(Convert(message));
        }

        public IReceiveSession StartReceiving(SysExMessageHandler receiveHandler)
        {
            return new SanfordReceiveSession(_input, receiveHandler);
        }

        private static San.SysExMessage Convert(SysExMessage message)
        {
            return new San.SysExMessage(message.Bytes);
        }

        private static SysExMessage Convert(San.SysExMessage message)
        {
            return new SysExMessage(message.GetBytes());
        }

        public class SanfordReceiveSession : IReceiveSession
        {
            private readonly InputDevice _input;
            private readonly EventHandler<SysExMessageEventArgs> _sanfordMessageHandler;

            public SanfordReceiveSession(InputDevice input, SysExMessageHandler receiveHandler)
            {
                _input = input;
                _sanfordMessageHandler = (_, e) =>
                {
                    receiveHandler(Convert(e.Message));
                };

                _input.SysExMessageReceived += _sanfordMessageHandler;
                _input.StartRecording();
            }

            public void Dispose()
            {
                _input.StopRecording();
                _input.SysExMessageReceived -= _sanfordMessageHandler;
            }
        }
    }
}