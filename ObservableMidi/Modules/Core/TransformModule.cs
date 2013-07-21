namespace ObservableMidi.Modules.Core
{
    public abstract class TransformModule<TIn, TOut> : Module, ISink<TIn>, ISource<TOut>
    {
        private readonly InputNode<TIn> _inputNode = new InputNode<TIn>();
        private readonly OutputNode<TOut> _outputNode = new OutputNode<TOut>();

        public InputNode<TIn> In { get { return _inputNode; } }
        public OutputNode<TOut> Out { get { return _outputNode; } }

        protected TransformModule()
        {
            _inputNode.Receive += OnReceive;
        }

        protected abstract void OnReceive(TIn data);

        protected void Send(TOut data)
        {
            Out.Send(data);
        }
    }
}
