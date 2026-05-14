using ZeroNull.Types.Option;

namespace ZeroNull.Tests.Types.Option
{
    public class OptionTests
    {
        [Fact]
        public void Some_CreatesOptionWithValue()
        {
            var option = Option<string>.Some("test");
            Assert.True(option.IsSome);
            Assert.False(option.IsNone);
            Assert.Equal("test", option.Value);
        }

        [Fact]
        public void Some_ThrowsWhenValueIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => Option<string>.Some(null!));
        }

        [Fact]
        public void None_CreatesOptionWithNoValue()
        {
            var option = Option<int>.None();
            Assert.True(option.IsNone);
            Assert.False(option.IsSome);
            Assert.Throws<InvalidOperationException>(() => option.Value);
        }

        [Fact]
        public void ValueOr_ReturnsValueWhenSome()
        {
            var option = Option<int>.Some(42);
            Assert.Equal(42, option.ValueOr(100));
        }

        [Fact]
        public void ValueOr_ReturnsFallbackWhenNone()
        {
            var option = Option<int>.None();
            Assert.Equal(100, option.ValueOr(100));
        }

        [Fact]
        public void ValueOr_Factory_ReturnsValueWhenSome()
        {
            var option = Option<string>.Some("hello");
            Assert.Equal("hello", option.ValueOr(() => "fallback"));
        }

        [Fact]
        public void ValueOr_Factory_ReturnsFactoryResultWhenNone()
        {
            var option = Option<string>.None();
            Assert.Equal("fallback", option.ValueOr(() => "fallback"));
        }

        [Fact]
        public void Map_TransformsSomeValue()
        {
            var option = Option<int>.Some(5);
            var result = option.Map(x => x * 2);
            Assert.True(result.IsSome);
            Assert.Equal(10, result.Value);
        }

        [Fact]
        public void Map_ReturnsNoneWhenNone()
        {
            var option = Option<int>.None();
            var result = option.Map(x => x * 2);
            Assert.True(result.IsNone);
        }

        [Fact]
        public void Bind_ReturnsSomeWhenSomeAndBinderReturnsSome()
        {
            var option = Option<int>.Some(5);
            var result = option.Bind(x => Option<int>.Some(x * 2));
            Assert.True(result.IsSome);
            Assert.Equal(10, result.Value);
        }

        [Fact]
        public void Bind_ReturnsNoneWhenSomeButBinderReturnsNone()
        {
            var option = Option<int>.Some(5);
            var result = option.Bind(x => Option<int>.None());
            Assert.True(result.IsNone);
        }

        [Fact]
        public void Bind_ReturnsNoneWhenNone()
        {
            var option = Option<int>.None();
            var result = option.Bind(x => Option<int>.Some(x * 2));
            Assert.True(result.IsNone);
        }

        [Fact]
        public void Match_CallsSomeDelegateWhenSome()
        {
            var option = Option<int>.Some(10);
            var result = option.Match(
                some: x => $"Value: {x}",
                none: () => "No value"
            );
            Assert.Equal("Value: 10", result);
        }

        [Fact]
        public void Match_CallsNoneDelegateWhenNone()
        {
            var option = Option<int>.None();
            var result = option.Match(
                some: x => $"Value: {x}",
                none: () => "No value"
            );
            Assert.Equal("No value", result);
        }

        [Fact]
        public void Linq_Select_ProjectsValue()
        {
            var result = from x in Option<int>.Some(5)
                         select x * 3;
            Assert.True(result.IsSome);
            Assert.Equal(15, result.Value);
        }

        [Fact]
        public void Linq_Select_ReturnsNoneWhenNone()
        {
            var result = from x in Option<int>.None()
                         select x * 3;
            Assert.True(result.IsNone);
        }

        [Fact]
        public void Linq_SelectMany_CombinesTwoOptions()
        {
            var result = from a in Option<int>.Some(2)
                         from b in Option<int>.Some(3)
                         select a + b;
            Assert.True(result.IsSome);
            Assert.Equal(5, result.Value);
        }

        [Fact]
        public void Linq_SelectMany_ShortCircuitsOnFirstNone()
        {
            var result = from a in Option<int>.Some(2)
                         from b in Option<int>.None()
                         select a + b;
            Assert.True(result.IsNone);
        }

        [Fact]
        public void Linq_SelectMany_WithProjection()
        {
            var result = from a in Option<string>.Some("Hello")
                         from b in Option<string>.Some("World")
                         select $"{a} {b}";
            Assert.Equal("Hello World", result.Value);
        }

        [Fact]
        public void Some_AllowsValueTypesEvenWithDefault()
        {
            var option = Option<int>.Some(0);
            Assert.True(option.IsSome);
            Assert.Equal(0, option.Value);
        }

        [Fact]
        public void None_WithReferenceType_ThrowsOnValueAccess()
        {
            var option = Option<string>.None();
            Assert.Throws<InvalidOperationException>(() => option.Value);
        }

        [Fact]
        public void Map_WithNullMapper_Throws()
        {
            var option = Option<int>.Some(5);
            Assert.Throws<ArgumentNullException>(() => option.Map<int>(null!));
        }

        [Fact]
        public void Bind_WithNullBinder_Throws()
        {
            var option = Option<int>.Some(5);
            Assert.Throws<ArgumentNullException>(() => option.Bind<int>(null!));
        }

        [Fact]
        public void Match_WithNullSomeDelegate_Throws()
        {
            var option = Option<int>.Some(5);
            Assert.Throws<ArgumentNullException>(() => option.Match<int>(null!, () => 0));
        }

        [Fact]
        public void Match_WithNullNoneDelegate_Throws()
        {
            var option = Option<int>.Some(5);
            Assert.Throws<ArgumentNullException>(() => option.Match<int>(x => x, null!));
        }

        [Fact]
        public void Chain_MapThenBindThenMatch()
        {
            var result = Option<int>.Some(4)
                .Map(x => x * 2)           // 8
                .Bind(x => x > 10 ? Option<int>.None() : Option<int>.Some(x))
                .Match(
                    some: x => $"Result: {x}",
                    none: () => "Too large"
                );
            Assert.Equal("Result: 8", result);
        }

        [Fact]
        public void Chain_WithFallback()
        {
            var final = Option<string>.None()
                .Map(s => s.ToUpper())
                .ValueOr("default");
            Assert.Equal("default", final);
        }
    }
}
