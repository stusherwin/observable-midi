namespace ObservableMidi.Modules.Core
{
    public abstract class SourceModule<T> : Module, ISource<T>
    {
        private readonly OutputNode<T> _outputNode = new OutputNode<T>();

        public OutputNode<T> Out { get { return _outputNode; } }

        public SourceModule()
        {
            _outputNode.Connected += (sender, args) => OnOutputConnected();
            _outputNode.Disconnected += (sender, args) => OnOutputDisconnected();
        }

        protected virtual void OnOutputConnected()
        {
        }

        protected virtual void OnOutputDisconnected()
        {
        }

        protected void Send(T message)
        {
            Out.Send(message);
        }
    }
}
