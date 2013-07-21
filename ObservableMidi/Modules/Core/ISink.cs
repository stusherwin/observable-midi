namespace ObservableMidi.Modules.Core
{
    public interface ISink<T>
    {
        InputNode<T> In { get; }
    }
}