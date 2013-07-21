namespace ObservableMidi.Modules.Core
{
    public interface ISource<T>
    {
        OutputNode<T> Out { get; }
    }
}