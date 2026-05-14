using ZeroNull.Types.Either;

namespace ZeroNull.Extensions
{
    public static class EitherExtensions
    {
        /// <summary>
        /// Allows to select values on a collection based on a custom comparator
        /// </summary>
        public static Either<TLeft, TResult> Select<TLeft, TRight, TResult>(
            this Either<TLeft, TRight> either,
            Func<TRight, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            if (!either.IsLeft)
                return Either<TLeft, TResult>.Of(selector(either.Right));

            return Either<TLeft, TResult>.Of(either.Left);
        }

        /// <summary>
        /// Allows to select many values on different collections based on a custom comparator
        /// </summary>
        public static Either<TLeft, TResult> SelectMany<TLeft, TRight, TCollection, TResult>(
            this Either<TLeft, TRight> either,
            Func<TRight, Either<TLeft, TCollection>> collectionSelector,
            Func<TRight, TCollection, TResult> resultSelector)
        {
            if (collectionSelector == null) throw new ArgumentNullException(nameof(collectionSelector));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

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

        /// <summary>
        /// Allows to match with Right value based on a custom comparator and a fallback value in case is false.
        /// </summary>
        public static Either<TLeft, TRight> Where<TLeft, TRight>(
              this Either<TLeft, TRight> either,
              Func<TRight, bool> predicate,
              TLeft leftOnFalse)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            if (either.IsLeft)
                return either;

            return predicate(either.Right) ? either : Either<TLeft, TRight>.Of(leftOnFalse);
        }

        /// <summary>
        /// Allows to match with Left value based on a custom comparator and a fallback value in case is false.
        /// </summary>
        public static Either<TLeft, TRight> Where<TLeft, TRight>(
          this Either<TLeft, TRight> either,
          Func<TLeft, bool> predicate,
          TLeft rightOnFalse)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            if (!either.IsLeft)
                return either;

            return predicate(either.Left) ? either : Either<TLeft, TRight>.Of(rightOnFalse);
        }

        /// <summary>
        /// Maps the Left value to a new type, leaving Right unchanged.
        /// </summary>
        public static Either<TLeft2, TRight> MapLeft<TLeft, TRight, TLeft2>(
            this Either<TLeft, TRight> either, 
            Func<TLeft, TLeft2> mapper)
        {
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));

            return either.IsLeft
                ? Either<TLeft2, TRight>.Of(mapper(either.Left))
                : Either<TLeft2, TRight>.Of(either.Right);
        }

        /// <summary>
        /// Maps the Right value to a new type, leaving Left unchanged.
        /// </summary>
        public static Either<TLeft, TRight2> MapRight<TLeft, TRight, TRight2>(
            this Either<TLeft, TRight> either,
            Func<TRight, TRight2> mapper)
        {
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));

            return !either.IsLeft
                ? Either<TLeft, TRight2>.Of(mapper(either.Right))
                : Either<TLeft, TRight2>.Of(either.Left);
        }

        /// <summary>
        /// Binds over the Left value, allowing transformation to a new Either (Left or Right).
        /// </summary>
        public static Either<TLeft2, TRight> BindLeft<TLeft, TRight, TLeft2>(
            this Either<TLeft, TRight> either,
            Func<TLeft, Either<TLeft2, TRight>> binder)
        {
            if (binder == null) throw new ArgumentNullException(nameof(binder));

            return either.IsLeft ? binder(either.Left) : Either<TLeft2, TRight>.Of(either.Right);
        }

        /// <summary>
        /// Binds over the Right value, allowing transformation to a new Either (Left or Right).
        /// </summary>
        public static Either<TLeft, TRight2> BindRight<TLeft, TRight, TRight2>(
            this Either<TLeft, TRight> either,
            Func<TRight, Either<TLeft, TRight2>> binder)
        {
            if (binder == null) throw new ArgumentNullException(nameof(binder));

            return !either.IsLeft ? binder(either.Right) : Either<TLeft, TRight2>.Of(either.Left);
        }
    }
}
