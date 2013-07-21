using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SetupManager.Core
{
    public interface IMidiDeviceFactory
    {
        IMidiDevice CreateMidiDevice(InputDeviceInfo inputDeviceInfo, OutputDeviceInfo outputDeviceInfo);
    }
}
