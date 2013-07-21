using Sanford.Multimedia.Midi;
using SetupManager.Core;

namespace SetupManager
{
    public class SanfordOutputDeviceInfoProvider : IDeviceInfoProvider<OutputDeviceInfo>
    {
        public OutputDeviceInfo[] GetDeviceInfos()
        {
            var result = new OutputDeviceInfo[OutputDevice.DeviceCount];

            for (int i = 0; i < OutputDevice.DeviceCount; i++)
            {
                result[i] = new OutputDeviceInfo
                {
                    DeviceId = i,
                    Name = OutputDevice.GetDeviceCapabilities(i).name
                };
            }

            return result;
        }
    }
}