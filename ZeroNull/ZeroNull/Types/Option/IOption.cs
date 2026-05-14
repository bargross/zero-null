namespace ZeroNull.Types.Option
{
    public interface IOption<T>
    {
        bool IsSome { get; }
        bool IsNone { get; }

        Option<U> Map<U>(Func<T, U> mapper);
        Option<U> Bind<U>(Func<T, Option<U>> binder);

        T ValueOr(T fallback); 
        T ValueOr(Func<T> fallbackFactory);

        TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none);
        Option<U> Select<U>(Func<T, U> mapper);
        Option<V> SelectMany<U, V>(Func<T, Option<U>> bind, Func<T, U, V> project);
    }
}
