using System;
using System.Collections.Generic;
using System.Linq;
using SetupManager.Core;

namespace SetupManager
{
    public class ConsoleSetupReader
    {
        private readonly IMidiDeviceFactory _midiDeviceFactory;
        private readonly IDeviceInfoProvider<InputDeviceInfo> _inputDeviceInfoProvider;
        private readonly IDeviceInfoProvider<OutputDeviceInfo> _outputDeviceInfoProvider;

        public ConsoleSetupReader(IMidiDeviceFactory midiDeviceFactory, IDeviceInfoProvider<InputDeviceInfo> inputDeviceInfoProvider, IDeviceInfoProvider<OutputDeviceInfo> outputDeviceInfoProvider)
        {
            _midiDeviceFactory = midiDeviceFactory;
            _inputDeviceInfoProvider = inputDeviceInfoProvider;
            _outputDeviceInfoProvider = outputDeviceInfoProvider;
        }

        public void ReadSetups()
        {
            var inputDeviceInfo = _inputDeviceInfoProvider.GetDeviceInfos().FirstOrDefault(d => d.DeviceId == 0);
            Console.WriteLine("Input device: {0}", inputDeviceInfo.Name);

            var outputDeviceInfo = _outputDeviceInfoProvider.GetDeviceInfos().FirstOrDefault(d => d.DeviceId == 1);
            Console.WriteLine("Output device: {0}", outputDeviceInfo.Name);

            List<Setup> setups;
            using (var device = _midiDeviceFactory.CreateMidiDevice(inputDeviceInfo, outputDeviceInfo))
            {
                var setupReceiver = new SetupReceiver(device);
                setups = LoadSetups(setupReceiver);
            }

            Console.WriteLine("Loaded {0} setups!", setups.Count);
            Console.WriteLine("Writing to file...");

            var writer = new XmlSetupWriter();
            writer.Write(setups, "setups.xml");

            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        private static List<Setup> LoadSetups(SetupReceiver setupReceiver)
        {
            var setups = new List<Setup>();

            Console.WriteLine();
            Console.WriteLine("Ready to load setups.");
            while (WaitForAnyKey("Choose a setup on the RD and press any key (or Esc to finish loading setups)", ConsoleKey.Escape))
            {
                Console.WriteLine();
                Console.WriteLine("Loading...");
                var setup = setupReceiver.RequestSetup();
                Console.WriteLine("Loaded setup: " + setup.Name);

                setups.Add(setup);
            }

            return setups;
        }

        private static bool WaitForAnyKey(string prompt, ConsoleKey endInput)
        {
            Console.WriteLine();
            Console.Write(prompt + ": ");
            return Console.ReadKey().Key != endInput;
        }
    }
}
