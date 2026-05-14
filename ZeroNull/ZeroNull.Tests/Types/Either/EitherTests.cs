using ZeroNull.Types.Either;

namespace ZeroNull.Tests.Types.Either
{
    public class EitherTests
    {
        [Fact]
        public void Of_WithLeftValue_CreatesLeftEither()
        {
            var either = Either<string, int>.Of("error");
            Assert.True(either.IsLeft);
            Assert.False(!either.IsLeft); // IsRight
            Assert.Equal("error", either.Left);
        }

        [Fact]
        public void Of_WithRightValue_CreatesRightEither()
        {
            var either = Either<string, int>.Of(42);
            Assert.False(either.IsLeft);
            Assert.True(!either.IsLeft); // IsRight
            Assert.Equal(42, either.Right);
        }

        [Fact]
        public void ImplicitOperator_FromLeft_CreatesLeftEither()
        {
            Either<string, int> either = "error";
            Assert.True(either.IsLeft);
            Assert.Equal("error", either.Left);
        }

        [Fact]
        public void ImplicitOperator_FromRight_CreatesRightEither()
        {
            Either<string, int> either = 42;
            Assert.False(either.IsLeft);
            Assert.Equal(42, either.Right);
        }

        [Fact]
        public void Left_WhenLeft_ReturnsValue()
        {
            var either = Either<string, int>.Of("test");
            Assert.Equal("test", either.Left);
        }

        [Fact]
        public void Left_WhenRight_ThrowsInvalidOperationException()
        {
            var either = Either<string, int>.Of(42);
            Assert.Throws<InvalidOperationException>(() => either.Left);
        }

        [Fact]
        public void Right_WhenRight_ReturnsValue()
        {
            var either = Either<string, int>.Of(99);
            Assert.Equal(99, either.Right);
        }

        [Fact]
        public void Right_WhenLeft_ThrowsInvalidOperationException()
        {
            var either = Either<string, int>.Of("error");
            Assert.Throws<InvalidOperationException>(() => either.Right);
        }

        [Fact]
        public void GetValue_WithMatchingType_WhenLeft_ReturnsLeftValue()
        {
            var either = Either<string, int>.Of("hello");
            var value = either.GetValue<string>();
            Assert.Equal("hello", value);
        }

        [Fact]
        public void GetValue_WithMatchingType_WhenRight_ReturnsRightValue()
        {
            var either = Either<string, int>.Of(123);
            var value = either.GetValue<int>();
            Assert.Equal(123, value);
        }

        [Fact]
        public void GetValue_WithNonMatchingType_ThrowsInvalidCastException()
        {
            var either = Either<string, int>.Of("test");
            Assert.Throws<InvalidCastException>(() => either.GetValue<int>());
        }

        [Fact]
        public void GetValue_WithWrongSideType_ThrowsInvalidCastException()
        {
            var either = Either<string, int>.Of(42);
            Assert.Throws<InvalidCastException>(() => either.GetValue<string>());
        }

        [Fact]
        public void ExplicitCast_ToLeftType_WhenLeft_ReturnsValue()
        {
            var either = Either<string, int>.Of("cast");
            string value = (string)either;
            Assert.Equal("cast", value);
        }

        [Fact]
        public void ExplicitCast_ToLeftType_WhenRight_ThrowsInvalidCastException()
        {
            var either = Either<string, int>.Of(5);
            Assert.Throws<InvalidCastException>(() => (string)either);
        }

        [Fact]
        public void ExplicitCast_ToRightType_WhenRight_ReturnsValue()
        {
            var either = Either<string, int>.Of(100);
            int value = (int)either;
            Assert.Equal(100, value);
        }

        [Fact]
        public void ExplicitCast_ToRightType_WhenLeft_ThrowsInvalidCastException()
        {
            var either = Either<string, int>.Of("error");
            Assert.Throws<InvalidCastException>(() => (int)either);
        }

        [Fact]
        public void Of_WithNullReferenceLeft_AllowsNull()
        {
            var either = Either<string, int>.Of(0);

            Assert.True(!either.IsLeft); // is right
            Assert.Null(either.LeftRawValue);
        }

        [Fact]
        public void Of_WithNullReferenceRight_AllowsNull()
        {
            var either = Either<string, int?>.Of(""); // right needs to be a nullable, primitive int defaults to 0

            Assert.True(either.IsLeft); // is left
            Assert.Null(either.RightRawValue);
        }

        [Fact]
        public void Dispose_DoesNotThrow()
        {
            var either = Either<string, int>.Of("test");
            var exception = Record.Exception(() => either.Dispose());

            Assert.Null(exception);
        }

        [Fact]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            var either = Either<string, int>.Of(42);

            either.Dispose();
            either.Dispose(); // Should not throw
        }

        [Fact]
        public void Either_WithDifferentTypes_Works()
        {
            var eitherInt = Either<string, int>.Of(10);
            var eitherBool = Either<string, bool>.Of(true);
            Assert.Equal(10, eitherInt.Right);
            Assert.True(eitherBool.Right);
        }
    }
}