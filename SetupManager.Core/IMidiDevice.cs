using System;

namespace SetupManager.Core
{
    public interface IMidiDevice : IDisposable
    {
        void Send(SysExMessage message);
        IReceiveSession StartReceiving(SysExMessageHandler receiveHandler);
    }

    public interface IReceiveSession : IDisposable
    {
    }
}
