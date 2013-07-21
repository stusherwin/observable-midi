using Sanford.Multimedia.Midi;
using SetupManager.Core;

namespace SetupManager
{
    public class SanfordInputDeviceInfoProvider : IDeviceInfoProvider<InputDeviceInfo>
    {
        public InputDeviceInfo[] GetDeviceInfos()
        {
            var result = new InputDeviceInfo[InputDevice.DeviceCount];

            for (int i = 0; i < InputDevice.DeviceCount; i++)
            {
                result[i] = new InputDeviceInfo
                {
                    DeviceId = i, 
                    Name = InputDevice.GetDeviceCapabilities(i).name
                };
            }

            return result;
        }
    }
}
