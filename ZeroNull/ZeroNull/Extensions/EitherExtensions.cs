using System;
using System.Collections.Generic;
using ZeroNull.Types.Either;

namespace ZeroNull.Extensions
{
    public static class EitherExtensions
    {
        // Select = Map (projects the Right side)
        public static Either<TLeft, TResult> Select<TLeft, TRight, TResult>(
            this Either<TLeft, TRight> either,
            Func<TRight, TResult> selector)
        {
            if (!either.IsLeft)
                return Either<TLeft, TResult>.Of(selector(either.Right));

            return Either<TLeft, TResult>.Of(either.Left);
        }

        public static Either<TLeft, TResult> SelectMany<TLeft, TRight, TCollection, TResult>(
            this Either<TLeft, TRight> either,
            Func<TRight, Either<TLeft, TCollection>> collectionSelector,
            Func<TRight, TCollection, TResult> resultSelector)
        {
            // Short-circuit: if this either is Left, propagate the error
            if (either.IsLeft)
                return Either<TLeft, TResult>.Of(either.Left);

            // Get the next monad in the chain
            var next = collectionSelector(either.Right);

            // If the next monad is Left, propagate the error
            if (next.IsLeft)
                return Either<TLeft, TResult>.Of(next.Left);

            // Both are Right, combine the values
            var result = resultSelector(either.Right, next.Right);
            return Either<TLeft, TResult>.Of(result);
        }

        public static Either<TLeft, TRight> Where<TLeft, TRight>(
              this Either<TLeft, TRight> either,
              Func<TRight, bool> predicate,
              TLeft leftOnFalse)
        {
            if (either.IsLeft)
                return either;

            return predicate(either.Right) ? either : Either<TLeft, TRight>.Of(leftOnFalse);
        }
    }
}
