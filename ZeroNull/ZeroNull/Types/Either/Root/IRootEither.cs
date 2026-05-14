namespace ZeroNull.Types.Either.Root
{
    internal interface IRootEither<TLeft, TRight>
    {
        TRight Right { get; }
        TLeft Left { get; }

        RootEither<TLeft, TRight> Of(TLeft left);
        RootEither<TLeft, TRight> Of(TRight right);
    }
}
