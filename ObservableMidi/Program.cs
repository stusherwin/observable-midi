using System;
using ObservableMidi.Modules;

namespace ObservableMidi
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var midiInput = MidiInput.FromDeviceId(0))
                using (var chordinator = new Chordinator())
                using (var chordOutputter = new ChordOutputter())
                using (var midiOutput = MidiOutput.FromDeviceId(1))
                {
                    midiInput.Out.ConnectTo(chordinator.In);
                    midiInput.Out.ConnectTo(midiOutput.In);
                    chordinator.Out.ConnectTo(chordOutputter.In);
                    chordOutputter.Out.ConnectTo(midiOutput.In);

                    Console.ReadLine();

                    midiInput.Out.DisconnectFrom(midiOutput.In);

                    Console.ReadLine();
                }
            }
            catch(Exception ex)
            {
                ReportError(ex);
            }
        }

        private static void ReportError(Exception ex)
        {
            Console.WriteLine("An error has occurred:");
            Console.WriteLine();
            Console.WriteLine(ex);
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
