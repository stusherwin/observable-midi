using SetupManager.Core;

namespace SetupManager
{
    public class Program
    {
        private static void Main(string[] args)
        {
            IMidiDeviceFactory factory = new SanfordMidiDeviceFactory();
            IDeviceInfoProvider<InputDeviceInfo> inputDeviceInfoProvider = new SanfordInputDeviceInfoProvider();
            IDeviceInfoProvider<OutputDeviceInfo> outputDeviceInfoProvider = new SanfordOutputDeviceInfoProvider();

            new ConsoleSetupManager(factory, inputDeviceInfoProvider, outputDeviceInfoProvider).Manage();
        }
    }
}