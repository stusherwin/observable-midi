namespace ObservableMidi.Modules.Core
{
    public abstract class SinkModule<T> : Module, ISink<T>
    {
        private readonly InputNode<T> _inputNode = new InputNode<T>();

        public InputNode<T> In { get { return _inputNode; } }

        protected SinkModule()
        {
            _inputNode.Receive += OnReceive;
        }

        protected abstract void OnReceive(T data);
    }
}
