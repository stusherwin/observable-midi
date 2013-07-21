using System;

namespace ObservableMidi.Modules.Core
{
    public class InputNode<T>
    {
        public event Action<T> Receive;

        public void OnReceive(T data)
        {
            if (Receive != null)
                Receive(data);
        }
    }
}