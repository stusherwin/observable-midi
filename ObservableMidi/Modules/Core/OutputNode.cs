using System;
using System.Collections.Generic;

namespace ObservableMidi.Modules.Core
{
    public class OutputNode<T>
    {
        private readonly List<InputNode<T>> _inputNodes = new List<InputNode<T>>();

        public event EventHandler<EventArgs> Connected;
        public event EventHandler<EventArgs> Disconnected;

        public void ConnectTo(InputNode<T> node)
        {
            _inputNodes.Add(node);

            if (_inputNodes.Count == 1 && Connected != null)
                Connected(this, EventArgs.Empty);

        }

        public void DisconnectFrom(InputNode<T> node)
        {
            _inputNodes.Remove(node);

            if (_inputNodes.Count == 0 && Disconnected != null)
                Disconnected(this, EventArgs.Empty);
        }

        public void Send(T data)
        {
            _inputNodes.ForEach(n => n.OnReceive(data));
        }
    }
}