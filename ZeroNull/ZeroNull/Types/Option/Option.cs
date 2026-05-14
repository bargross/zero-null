namespace ZeroNull.Types.Option
{
    using System;

    /// <summary>
    /// Represents an optional value that can be either Some (has a value) or None (missing).
    /// </summary>
    public readonly struct Option<T>: IOption<T>
    {
        private readonly T _value;
        private readonly bool _hasValue;

        private Option(T value, bool hasValue)
        {
            _value = value;
            _hasValue = hasValue;
        }

        public static Option<T> Some(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            return new Option<T>(value, true);
        }

        public static Option<T> None() => new Option<T>(default!, false);

        public bool IsSome => _hasValue;
        public bool IsNone => !_hasValue;

        public T Value =>
            IsSome ? _value! : throw new InvalidOperationException("Option has no value.");

        // Core functional methods
        public Option<U> Map<U>(Func<T, U> mapper)
        {
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));
            return IsSome ? Option<U>.Some(mapper(_value!)) : Option<U>.None();
        }

        public Option<U> Bind<U>(Func<T, Option<U>> binder)
        {
            if (binder == null) throw new ArgumentNullException(nameof(binder));
            return IsSome ? binder(_value!) : Option<U>.None();
        }

        public T ValueOr(T fallback) {
            if (fallback == null)
                throw new ArgumentNullException(nameof(fallback));

            return IsSome? _value! : fallback;
        }

        public T ValueOr(Func<T> fallbackFactory)
        {
            if (fallbackFactory == null) throw new ArgumentNullException(nameof(fallbackFactory));

            return IsSome ? _value! : fallbackFactory();
        }

        // Exhaustive matching
        public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
        {
            if (some == null) throw new ArgumentNullException(nameof(some));
            if (none == null) throw new ArgumentNullException(nameof(none));
            return IsSome ? some(_value!) : none();
        }

        // LINQ support (optional but nice)
        public Option<U> Select<U>(Func<T, U> mapper) => Map(mapper);

        public Option<V> SelectMany<U, V>(Func<T, Option<U>> bind, Func<T, U, V> project)
        {
            return Bind(x => bind(x).Map(y => project(x, y)));
        }
    }
}