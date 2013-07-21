using ObservableMidi.Modules.Core;
using ObservableMidi.Modules.Data;
using Sanford.Multimedia.Midi;

namespace ObservableMidi.Modules
{
    public class MidiOutput : SinkModule<Note>
    {
        private readonly OutputDevice _outputDevice;

        public string DeviceName { get; private set; }

        public MidiOutput(OutputDevice outputDevice, string deviceName)
        {
            _outputDevice = outputDevice;
            DeviceName = deviceName;
        }

        public override void Dispose()
        {
            _outputDevice.Close();
            _outputDevice.Dispose();
        }

        protected override void OnReceive(Note data)
        {
            _outputDevice.Send(data.Message);
        }

        public static MidiOutput FromDeviceId(int deviceId)
        {
            return new MidiOutput(
                new OutputDevice(deviceId), 
                OutputDeviceBase.GetDeviceCapabilities(deviceId).name);
        }
    }
}