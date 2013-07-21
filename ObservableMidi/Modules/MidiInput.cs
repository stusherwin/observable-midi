using System;
using System.Reactive;
using System.Reactive.Linq;
using ObservableMidi.Modules.Core;
using ObservableMidi.Modules.Data;
using Sanford.Multimedia.Midi;

namespace ObservableMidi.Modules
{
    public class MidiInputObservable : SourceModule<Note>
    {
        private readonly InputDevice _inputDevice;
        private IObservable<EventPattern<ChannelMessageEventArgs>> _observable;

        public string DeviceName { get; private set; }

        public MidiInputObservable(InputDevice inputDevice, string deviceName)
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
            _observable = Observable.FromEventPattern<ChannelMessageEventArgs>(
                h => _inputDevice.ChannelMessageReceived += h, 
                h => _inputDevice.ChannelMessageReceived -= h);
            _observable.Subscribe(e => OnChannelMessageReceived(e.Sender, e.EventArgs));
            
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

        public static MidiInputObservable FromDeviceId(int deviceId)
        {
            return new MidiInputObservable(
                new InputDevice(deviceId), 
                InputDevice.GetDeviceCapabilities(deviceId).name);
        }
    }
}