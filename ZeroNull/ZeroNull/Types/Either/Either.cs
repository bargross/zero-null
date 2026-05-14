using ZeroNull.Types.Either.Root;

namespace ZeroNull.Types.Either
{
    public class Either<TLeft, TRight> : IEither<TLeft, TRight>
    {
        private RootEither<TLeft, TRight> _root;
        private Type _currentType;
        private bool _disposed;

        public bool IsLeft => _root.LeftPresent;

        public TLeft Left => IsLeft ? _root.Left! : throw new InvalidOperationException("Either is Right, no Left value.");
        public TRight Right => !IsLeft ? _root.Right! : throw new InvalidOperationException("Either is Left, no Right value.");

        internal TLeft LeftRawValue => _root.RawLeft;
        internal TRight RightRawValue => _root.RawRight;

        private Either(TLeft left)
        {
            _root = new RootEither<TLeft, TRight>(left);

             _currentType = typeof(TLeft);
        }

        private Either(TRight right)
        {
            _root = new RootEither<TLeft, TRight>(right);

            _currentType = typeof(TRight);
        }

        ~Either() => Dispose(false);

        public T GetValue<T>()
        {
            var type = typeof(T);

            if (_currentType == type)
            {
                if (_root.LeftPresent) 
                    return (T)Convert.ChangeType(_root.Left, type);
                
                if (_root.RightPresent)
                    return (T)Convert.ChangeType(_root.Right, type);              
            }

            throw new InvalidCastException($"Either {typeof(TLeft)} nor {typeof(TRight)} match type: {typeof(T)}");
        }
        
        public static Either<TLeft, TRight> Of(TLeft value) => new Either<TLeft, TRight>(value);
        public static Either<TLeft, TRight> Of(TRight value) => new Either<TLeft, TRight>(value);

        // Assignment & Cast Operators

        public static implicit operator Either<TLeft, TRight>(TRight right) => new Either<TLeft, TRight>(right);
        public static implicit operator Either<TLeft, TRight>(TLeft left) => new Either<TLeft, TRight>(left);

        public static explicit operator TLeft(Either<TLeft, TRight> either) => either.GetValue<TLeft>();
        public static explicit operator TRight(Either<TLeft, TRight> either) => either.GetValue<TRight>();
        
        // IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!_disposed)
            {
                if (disposing) 
                {
                    //_ruleValidator.Dispose();
                    _root.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
