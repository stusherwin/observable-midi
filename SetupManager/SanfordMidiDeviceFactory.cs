using Sanford.Multimedia.Midi;
using SetupManager.Core;

namespace SetupManager
{
    public class SanfordMidiDeviceFactory : IMidiDeviceFactory
    {
        public IMidiDevice CreateMidiDevice(InputDeviceInfo inputDeviceInfo, OutputDeviceInfo outputDeviceInfo)
        {
            return new SanfordMidiDevice(
                new InputDevice(inputDeviceInfo.DeviceId),
                new OutputDevice(outputDeviceInfo.DeviceId)
            );
        }
    }
}