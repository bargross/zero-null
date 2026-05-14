using System;
using Xunit;
using ZeroNull.Extensions;
using ZeroNull.Types.Either;

namespace ZeroNull.Tests.Extensions
{
    public class EitherExtensionsTests
    {
        private static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft value) => Either<TLeft, TRight>.Of(value);
        private static Either<TLeft, TRight> Right<TLeft, TRight>(TRight value) => Either<TLeft, TRight>.Of(value);


        [Fact]
        public void Select_OnRight_ProjectsValue()
        {
            var either = Right<string, int>(5);
            var result = either.Select(x => x * 2);

            Assert.False(result.IsLeft);
            Assert.Equal(10, result.Right);
        }

        [Fact]
        public void Select_OnLeft_PropagatesLeft()
        {
            var either = Left<string, int>("error");
            var result = either.Select(x => x * 2);

            Assert.True(result.IsLeft);
            Assert.Equal("error", result.Left);
        }

        [Fact]
        public void Select_WithNullSelector_Throws()
        {
            var either = Right<string, int>(5);

            Assert.Throws<ArgumentNullException>(() => either.Select<string, int, int>(null!));
        }

        [Fact]
        public void SelectMany_OnRightRight_Combines()
        {
            var either = Right<string, int>(2);

            var result = either.SelectMany(
                x => Right<string, int>(x * 3),
                (a, b) => a + b
            );

            Assert.False(result.IsLeft);
            Assert.Equal(8, result.Right);
        }

        [Fact]
        public void SelectMany_FirstIsLeft_PropagatesLeft()
        {
            var either = Left<string, int>("first error");

            var result = either.SelectMany(
                x => Right<string, int>(x * 3),
                (a, b) => a + b
            );

            Assert.True(result.IsLeft);
            Assert.Equal("first error", result.Left);
        }

        [Fact]
        public void SelectMany_SecondIsLeft_PropagatesLeft()
        {
            var either = Right<string, int>(2);

            var result = either.SelectMany(
                x => Left<string, int>("second error"),
                (a, b) => a + b
            );

            Assert.True(result.IsLeft);
            Assert.Equal("second error", result.Left);
        }

        [Fact]
        public void SelectMany_WithNullCollectionSelector_Throws()
        {
            var either = Right<string, int>(2);

            Assert.Throws<ArgumentNullException>(() =>
                either.SelectMany<string, int, int, int>(null!, (a, b) => a + b));
        }

        [Fact]
        public void SelectMany_WithNullResultSelector_Throws()
        {
            var either = Right<string, int>(2);

            Assert.Throws<ArgumentNullException>(() =>
                either.SelectMany<string, int, int, int>(x => Right<string, int>(x), null!));
        }

        [Fact]
        public void Where_RightPredicate_True_ReturnsSameRight()
        {
            var either = Right<string, int>(10);

            var result = either.Where(x => x > 5, "too small");

            Assert.False(result.IsLeft);
            Assert.Equal(10, result.Right);
        }

        [Fact]
        public void Where_RightPredicate_False_ReturnsLeftWithError()
        {
            var either = Right<string, int>(3);

            var result = either.Where(x => x > 5, "too small");

            Assert.True(result.IsLeft);
            Assert.Equal("too small", result.Left);
        }

        [Fact]
        public void Where_RightPredicate_OnLeft_PropagatesLeft()
        {
            var either = Left<string, int>("original");

            var result = either.Where(x => x > 5, "too small");

            Assert.True(result.IsLeft);
            Assert.Equal("original", result.Left);
        }

        [Fact]
        public void Where_RightPredicate_WithNullPredicate_Throws()
        {
            var either = Right<string, int>(5);

            Assert.Throws<ArgumentNullException>(() => either.Where((Func<int, bool>)null!, "error"));
        }

        // ========== Where (Left predicate) ==========

        [Fact]
        public void Where_LeftPredicate_OnLeft_True_ReturnsSameLeft()
        {
            var either = Left<string, int>("keep");

            var result = either.Where((string s) => s == "keep", "fallback");

            Assert.True(result.IsLeft);
            Assert.Equal("keep", result.Left);
        }

        [Fact]
        public void Where_LeftPredicate_OnLeft_False_ReturnsLeftWithFallback()
        {
            var either = Left<string, int>("old");

            var result = either.Where((string s) => s == "keep", "fallback");

            Assert.True(result.IsLeft);
            Assert.Equal("fallback", result.Left);
        }

        [Fact]
        public void Where_LeftPredicate_OnRight_PropagatesRight()
        {
            var either = Right<string, int>(42);

            var result = either.Where((string s) => s == "keep", "fallback");

            Assert.False(result.IsLeft);
            Assert.Equal(42, result.Right);
        }

        [Fact]
        public void Where_LeftPredicate_WithNullPredicate_Throws()
        {
            var either = Left<string, int>("test");

            Assert.Throws<ArgumentNullException>(() => either.Where((Func<string, bool>)null!, "fallback"));
        }

        [Fact]
        public void MapLeft_OnLeft_TransformsValue()
        {
            var either = Left<string, int>("hello");

            var result = either.MapLeft(s => s.Length);

            Assert.True(result.IsLeft);
            Assert.Equal(5, result.Left);
        }

        [Fact]
        public void MapLeft_OnRight_PreservesRight()
        {
            var either = Right<string, int>(99);

            var result = either.MapLeft(s => s.Length);

            Assert.False(result.IsLeft);
            Assert.Equal(99, result.Right);
        }

        [Fact]
        public void MapLeft_WithNullMapper_Throws()
        {
            var either = Left<string, int>("test");

            Assert.Throws<ArgumentNullException>(() => either.MapLeft<string, int, int>(null!));
        }

        [Fact]
        public void MapRight_OnRight_TransformsValue()
        {
            var either = Right<string, int>(5);

            var result = either.MapRight(x => x * 3);

            Assert.False(result.IsLeft);
            Assert.Equal(15, result.Right);
        }

        [Fact]
        public void MapRight_OnLeft_PreservesLeft()
        {
            var either = Left<string, int>("error");

            var result = either.MapRight(x => x * 3);

            Assert.True(result.IsLeft);
            Assert.Equal("error", result.Left);
        }

        [Fact]
        public void MapRight_WithNullMapper_Throws()
        {
            var either = Right<string, int>(5);

            Assert.Throws<ArgumentNullException>(() => either.MapRight<string, int, int>(null!));
        }

        // ========== BindLeft ==========

        [Fact]
        public void BindLeft_OnLeft_AppliesBinder()
        {
            var either = Left<string, int>("start");

            var result = either.BindLeft(s => Left<int, int>(s.Length));

            Assert.True(result.IsLeft);
            Assert.Equal(5, result.Left);
        }

        [Fact]
        public void BindLeft_OnRight_PreservesRight()
        {
            var either = Right<string, int>(42);

            var result = either.BindLeft(s => Left<int, int>(s.Length));

            Assert.False(result.IsLeft);
            Assert.Equal(42, result.Right);
        }

        [Fact]
        public void BindLeft_WithNullBinder_Throws()
        {
            var either = Left<string, int>("x");

            Assert.Throws<ArgumentNullException>(() => either.BindLeft<string, int, int>(null!));
        }

        [Fact]
        public void BindRight_OnRight_AppliesBinder()
        {
            var either = Right<string, int>(10);

            var result = either.BindRight(x => Right<string, double>(x * 2.5));

            Assert.False(result.IsLeft);
            Assert.Equal(25.0, result.Right);
        }

        [Fact]
        public void BindRight_OnLeft_PreservesLeft()
        {
            var either = Left<string, int>("error");

            var result = either.BindRight(x => Right<string, double>(x * 2.5));

            Assert.True(result.IsLeft);
            Assert.Equal("error", result.Left);
        }

        [Fact]
        public void BindRight_WithNullBinder_Throws()
        {
            var either = Right<string, int>(5);

            Assert.Throws<ArgumentNullException>(() => either.BindRight<string, int, double>(null!));
        }


        [Fact]
        public void Chaining_MapRightThenWhere_Works()
        {
            var result = Right<string, int>(4)
                .MapRight(x => x * 2) 
                .Where(x => x > 10, "too low")
                .MapLeft(err => $"Error: {err}");

            Assert.True(result.IsLeft);
            Assert.Equal("Error: too low", result.Left);
        }

        [Fact]
        public void Chaining_SelectManyWithMapRight()
        {
            var final = from a in Right<string, int>(3)
                        from b in Right<string, int>(5)
                        select a + b;

            var mapped = final.MapRight(x => x * 2);

            Assert.False(mapped.IsLeft);
            Assert.Equal(16, mapped.Right);
        }
    }
}