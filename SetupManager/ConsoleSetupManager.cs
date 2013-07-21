using System;
using System.Collections.Generic;
using System.Linq;
using SetupManager.Core;

namespace SetupManager
{
    public class ConsoleSetupManager
    {
        private readonly IMidiDeviceFactory _midiDeviceFactory;
        private readonly IDeviceInfoProvider<InputDeviceInfo> _inputDeviceInfoProvider;
        private readonly IDeviceInfoProvider<OutputDeviceInfo> _outputDeviceInfoProvider;

        public ConsoleSetupManager(IMidiDeviceFactory midiDeviceFactory, IDeviceInfoProvider<InputDeviceInfo> inputDeviceInfoProvider, IDeviceInfoProvider<OutputDeviceInfo> outputDeviceInfoProvider)
        {
            _midiDeviceFactory = midiDeviceFactory;
            _inputDeviceInfoProvider = inputDeviceInfoProvider;
            _outputDeviceInfoProvider = outputDeviceInfoProvider;
        }

        public void Manage()
        {
            var inputDeviceInfo = GetDeviceInfo(_inputDeviceInfoProvider, "input");
            var outputDeviceInfo = GetDeviceInfo(_outputDeviceInfoProvider, "output");

            using (var device = _midiDeviceFactory.CreateMidiDevice(inputDeviceInfo, outputDeviceInfo))
            {
                var setupReceiver = new SetupReceiver(device);
                var setupSender = new SetupSender(device);
                var setups = LoadSetups(setupReceiver).ToArray();

                Console.WriteLine("Loaded setups:");
                for (int i = 0; i < setups.Length; i++)
                {
                    Console.WriteLine("{0}: {1}", i, setups[i].RDName);
                }

                Console.WriteLine();
                WhileIntInput("Choose a setup to load into the RD: (q to quit)", "q",
                              i => i >= 0 && i < setups.Length,
                              i => setupSender.Send(setups[i]));
            }
        }

        private static T GetDeviceInfo<T>(IDeviceInfoProvider<T> deviceInfoProvider, string type)
            where T : DeviceInfo
        {
            var deviceInfos = deviceInfoProvider.GetDeviceInfos();

            Console.WriteLine();
            foreach (var deviceInfo in deviceInfos)
            {
                Console.WriteLine("{0}: {1}", deviceInfo.DeviceId, deviceInfo.Name);
            }

            var id = WaitForInt("Choose " + type + " device", i => i >= 0 && i < deviceInfos.Length);
            return deviceInfos.First(i => i.DeviceId == id);
        }

        private static IEnumerable<Setup> LoadSetups(SetupReceiver setupReceiver)
        {
            Console.WriteLine();
            Console.WriteLine("Ready to load setups.");
            while (WaitForAnyKey("Choose a setup on the RD and press any key (or Esc to finish loading setups)", ConsoleKey.Escape))
            {
                var setup = setupReceiver.RequestSetup();
                Console.WriteLine("Loaded setup \"" + setup.Name + "\"");

                yield return setup;
            }
        }

        private static bool WaitForAnyKey(string prompt, ConsoleKey endInput)
        {
            Console.WriteLine();
            Console.Write(prompt + ": ");
            return Console.ReadKey().Key != endInput;
        }

        private static string WaitForInput(string prompt)
        {
            Console.WriteLine();
            Console.Write(prompt + ": ");
            return Console.ReadLine();
        }

        private static int WaitForInt(string prompt, Func<int, bool> valid)
        {
            int intInput;

            while (!int.TryParse(WaitForInput(prompt), out intInput) || !valid(intInput))
                ;

            return intInput;
        }

        private static void WhileIntInput(string prompt, string endInput, Func<int, bool> valid, Action<int> action)
        {
            string input;

            while ((input = WaitForInput(prompt)) != endInput)
            {
                int intInput;
                if (!int.TryParse(input, out intInput) || !valid(intInput))
                    continue;

                action(intInput);
            }
        }
    }
}