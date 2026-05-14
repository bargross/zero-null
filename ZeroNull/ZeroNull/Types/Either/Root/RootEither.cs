using System;

namespace ZeroNull.Types.Either.Root
{
    internal class RootEither<TLeft, TRight> : IRootEither<TLeft, TRight>, IDisposable
    {
        public bool LeftPresent { get; }
        public bool RightPresent { get; }

        private TLeft _left;
        private TRight _right;
        private bool _disposed = false;

        public TLeft Left
        {
            get
            {
                if (LeftPresent && !RightPresent)
                {
                    return _left;
                }

                throw new FieldAccessException("Value is not present");
            }
        }

        public TRight Right
        {
            get
            {
                if (RightPresent && !LeftPresent)
                {
                    return _right;
                }

                throw new FieldAccessException("Value is not present");
            }
        }

        public TLeft RawLeft => _left;
        public TRight RawRight => _right;

        public Type LeftType => typeof(TLeft);
        public Type RightType => typeof(TRight);

        public RootEither(TLeft left)
        {
            _left = left;
            _right = default;

            LeftPresent = true;
            RightPresent = false;
        }

        public RootEither(TRight right)
        {
            _right = right;
            _left = default;

            LeftPresent = false;
            RightPresent = true;
        }

        ~RootEither() => Dispose(false);
 
        public RootEither<TLeft, TRight> Of(TLeft left) => new RootEither<TLeft, TRight>(left);
        public RootEither<TLeft, TRight> Of(TRight right) => new RootEither<TLeft, TRight>(right);

        // assignment operators

        public static implicit operator RootEither<TLeft, TRight>(TLeft left) => new RootEither<TLeft, TRight>(left);
        public static implicit operator RootEither<TLeft, TRight>(TRight right) => new RootEither<TLeft, TRight>(right);


        public static explicit operator TLeft(RootEither<TLeft, TRight> value) => value.Left;
        public static explicit operator TRight(RootEither<TLeft, TRight> value) => value.Right;

        // GC

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
                    // managed resources

                    _left = default(TLeft);
                    _right =  default(TRight);
                }

                _disposed = true;
            }
        }
    }
}
