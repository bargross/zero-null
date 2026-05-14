

namespace ZeroNull.Types.Result
{
    /// <summary>
    /// Represents a computation that can either succeed with a value (Ok) or fail with an error (Error).
    /// </summary>
    public readonly struct Result<T, TError>
    {
        private readonly T? _value;
        private readonly TError? _error;
        private readonly bool _isOk;

        private Result(T? value, TError? error, bool isOk)
        {
            _value = value;
            _error = error;
            _isOk = isOk;
        }

        public static Result<T, TError> Ok(T value) =>
            new Result<T, TError>(value, default, true);

        public static Result<T, TError> Error(TError error) =>
            new Result<T, TError>(default, error, false);

        public bool IsOk => _isOk;
        public bool IsError => !_isOk;

        public T Value =>
            IsOk ? _value! : throw new InvalidOperationException("Cannot access value of an error result.");

        public TError ErrorValue =>
            IsError ? _error! : throw new InvalidOperationException("Cannot access error of a success result.");

        // Core functional methods
        public Result<U, TError> Map<U>(Func<T, U> mapper) =>
            IsOk ? Result<U, TError>.Ok(mapper(_value!)) : Result<U, TError>.Error(_error!);

        public Result<U, TError> Bind<U>(Func<T, Result<U, TError>> binder) =>
            IsOk ? binder(_value!) : Result<U, TError>.Error(_error!);

        public Result<T, TError2> MapError<TError2>(Func<TError, TError2> mapper) =>
            IsError ? Result<T, TError2>.Error(mapper(_error!)) : Result<T, TError2>.Ok(_value!);

        public T ValueOr(T fallback) => IsOk ? _value! : fallback;

        // Exhaustive matching
        public TResult Match<TResult>(Func<T, TResult> ok, Func<TError, TResult> error) =>
            IsOk ? ok(_value!) : error(_error!);

        // LINQ support
        public Result<U, TError> Select<U>(Func<T, U> mapper) => Map(mapper);
        public Result<V, TError> SelectMany<U, V>(
            Func<T, Result<U, TError>> bind,
            Func<T, U, V> project)
        {
            return Bind(x => bind(x).Map(y => project(x, y)));
        }
    }
}
