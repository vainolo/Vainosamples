public class ContextProvider<T> where T : new()
{
    public T InitContext()
    {
        _asyncLocal.Value = new T();
        return _asyncLocal.Value;
    }

    public T GetContext()
    {
        return _asyncLocal.Value;
    }

    private static readonly AsyncLocal<T> _asyncLocal = new AsyncLocal<T>();
}