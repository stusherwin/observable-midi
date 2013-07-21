using System.Threading;

namespace SetupManager.Core
{
    public class SetupSender
    {
        private readonly IMidiDevice _device;

        public SetupSender(IMidiDevice device)
        {
            _device = device;
        }

        public void Send(Setup setup)
        {
            foreach (var msg in setup.SysExMessages)
            {
                _device.Send(msg);
                Thread.Sleep(40);
            }
        }
    }
}