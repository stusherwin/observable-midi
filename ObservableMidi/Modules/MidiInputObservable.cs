using ObservableMidi.Modules.Core;
using ObservableMidi.Modules.Data;
using Sanford.Multimedia.Midi;

namespace ObservableMidi.Modules
{
    public class MidiInput : SourceModule<Note>
    {
        private readonly InputDevice _inputDevice;

        public string DeviceName { get; private set; }

        public MidiInput(InputDevice inputDevice, string deviceName)
        {
            _inputDevice = inputDevice;
            DeviceName = deviceName;
        }

        public override void Dispose()
        {
            StopListening();

            _inputDevice.Close();
            _inputDevice.Dispose();
        }

        protected override void OnOutputConnected()
        {
            StartListening();
        }

        protected override void OnOutputDisconnected()
        {
            StopListening();
        }

        private void StartListening()
        {
            _inputDevice.StartRecording();
            _inputDevice.ChannelMessageReceived += OnChannelMessageReceived;
        }

        private void StopListening()
        {
            _inputDevice.StopRecording();
            _inputDevice.ChannelMessageReceived -= OnChannelMessageReceived;
        }

        private void OnChannelMessageReceived(object sender, ChannelMessageEventArgs args)
        {
            Send(new Note { Message = args.Message });
        }

        public static MidiInput FromDeviceId(int deviceId)
        {
            return new MidiInput(
                new InputDevice(deviceId), 
                InputDevice.GetDeviceCapabilities(deviceId).name);
        }
    }
}