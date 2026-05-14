using ZeroNull.Types.Either.Rule;

namespace ZeroNull.Types.Either
{
    public interface IEither<TLeft, TRight>: IDisposable
    {
        TLeft Left { get; }
        TRight Right { get; }
        bool IsLeft { get; }

        T GetValue<T>();
    }
}
