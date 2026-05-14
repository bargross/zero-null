using ZeroNull.Types.Result;

namespace ZeroNull.Tests.Types.Result
{
    public class ResultTests
    {
        // ========== Creation & Properties ==========

        [Fact]
        public void Ok_CreatesResultWithValue()
        {
            var result = Result<int, string>.Ok(42);
            Assert.True(result.IsOk);
            Assert.False(result.IsError);
            Assert.Equal(42, result.Value);
            Assert.Throws<InvalidOperationException>(() => result.ErrorValue);
        }

        [Fact]
        public void Error_CreatesResultWithError()
        {
            var result = Result<int, string>.Error("Something went wrong");
            Assert.True(result.IsError);
            Assert.False(result.IsOk);
            Assert.Equal("Something went wrong", result.ErrorValue);
            Assert.Throws<InvalidOperationException>(() => result.Value);
        }

        [Fact]
        public void Ok_AllowsNullForReferenceTypeValue_IfTIsNullableReference()
        {
            // For reference types, null is a valid value (though maybe not desirable)
            var result = Result<string, string>.Ok(null!);
            Assert.True(result.IsOk);
            Assert.Null(result.Value);
        }

        [Fact]
        public void Error_AllowsNullForErrorType()
        {
            var result = Result<int, string>.Error(null!);
            Assert.True(result.IsError);
            Assert.Null(result.ErrorValue);
        }

        // ========== ValueOr ==========

        [Fact]
        public void ValueOr_ReturnsValueWhenOk()
        {
            var result = Result<int, string>.Ok(42);
            Assert.Equal(42, result.ValueOr(100));
        }

        [Fact]
        public void ValueOr_ReturnsFallbackWhenError()
        {
            var result = Result<int, string>.Error("fail");
            Assert.Equal(100, result.ValueOr(100));
        }

        // ========== Map ==========

        [Fact]
        public void Map_TransformsOkValue()
        {
            var result = Result<int, string>.Ok(5);
            var mapped = result.Map(x => x * 2);
            Assert.True(mapped.IsOk);
            Assert.Equal(10, mapped.Value);
        }

        [Fact]
        public void Map_PropagatesErrorWhenError()
        {
            var result = Result<int, string>.Error("original error");
            var mapped = result.Map(x => x * 2);
            Assert.True(mapped.IsError);
            Assert.Equal("original error", mapped.ErrorValue);
        }

        [Fact]
        public void Map_WithNullMapper_Throws()
        {
            var result = Result<int, string>.Ok(5);
            Assert.Throws<ArgumentNullException>(() => result.Map<int>(null!));
        }

        // ========== Bind ==========

        [Fact]
        public void Bind_TransformsOkToNewResult()
        {
            var result = Result<int, string>.Ok(5);
            var bound = result.Bind(x => Result<int, string>.Ok(x * 2));
            Assert.True(bound.IsOk);
            Assert.Equal(10, bound.Value);
        }

        [Fact]
        public void Bind_TransformsOkToError()
        {
            var result = Result<int, string>.Ok(5);
            var bound = result.Bind(x => Result<int, string>.Error("converted to error"));
            Assert.True(bound.IsError);
            Assert.Equal("converted to error", bound.ErrorValue);
        }

        [Fact]
        public void Bind_PropagatesErrorWhenError()
        {
            var result = Result<int, string>.Error("persistent error");
            var bound = result.Bind(x => Result<int, string>.Ok(x * 2));
            Assert.True(bound.IsError);
            Assert.Equal("persistent error", bound.ErrorValue);
        }

        [Fact]
        public void Bind_WithNullBinder_Throws()
        {
            var result = Result<int, string>.Ok(5);
            Assert.Throws<ArgumentNullException>(() => result.Bind<int>(null!));
        }

        // ========== MapError ==========

        [Fact]
        public void MapError_TransformsErrorWhenError()
        {
            var result = Result<int, string>.Error("error");
            var mapped = result.MapError(err => $"Prefix: {err}");
            Assert.True(mapped.IsError);
            Assert.Equal("Prefix: error", mapped.ErrorValue);
        }

        [Fact]
        public void MapError_PropagatesOkWhenOk()
        {
            var result = Result<int, string>.Ok(42);
            var mapped = result.MapError(err => $"Prefix: {err}");
            Assert.True(mapped.IsOk);
            Assert.Equal(42, mapped.Value);
        }

        [Fact]
        public void MapError_WithNullMapper_Throws()
        {
            var result = Result<int, string>.Error("error");
            Assert.Throws<ArgumentNullException>(() => result.MapError<string>(null!));
        }

        // ========== Match ==========

        [Fact]
        public void Match_CallsOkDelegateWhenOk()
        {
            var result = Result<int, string>.Ok(10);
            var output = result.Match(
                ok: x => $"Value: {x}",
                error: e => $"Error: {e}"
            );
            Assert.Equal("Value: 10", output);
        }

        [Fact]
        public void Match_CallsErrorDelegateWhenError()
        {
            var result = Result<int, string>.Error("failure");
            var output = result.Match(
                ok: x => $"Value: {x}",
                error: e => $"Error: {e}"
            );
            Assert.Equal("Error: failure", output);
        }

        [Fact]
        public void Match_WithNullOkDelegate_Throws()
        {
            var result = Result<int, string>.Ok(5);
            Assert.Throws<ArgumentNullException>(() => result.Match<int>(null!, e => 0));
        }

        [Fact]
        public void Match_WithNullErrorDelegate_Throws()
        {
            var result = Result<int, string>.Ok(5);
            Assert.Throws<ArgumentNullException>(() => result.Match<int>(x => x, null!));
        }

        // ========== LINQ Query Syntax ==========

        [Fact]
        public void Linq_Select_ProjectsOkValue()
        {
            var result = from x in Result<int, string>.Ok(5)
                         select x * 3;
            Assert.True(result.IsOk);
            Assert.Equal(15, result.Value);
        }

        [Fact]
        public void Linq_Select_PropagatesError()
        {
            var result = from x in Result<int, string>.Error("fail")
                         select x * 3;
            Assert.True(result.IsError);
            Assert.Equal("fail", result.ErrorValue);
        }

        [Fact]
        public void Linq_SelectMany_CombinesTwoResults()
        {
            var result = from a in Result<int, string>.Ok(2)
                         from b in Result<int, string>.Ok(3)
                         select a + b;
            Assert.True(result.IsOk);
            Assert.Equal(5, result.Value);
        }

        [Fact]
        public void Linq_SelectMany_ShortCircuitsOnFirstError()
        {
            var result = from a in Result<int, string>.Ok(2)
                         from b in Result<int, string>.Error("b error")
                         select a + b;
            Assert.True(result.IsError);
            Assert.Equal("b error", result.ErrorValue);
        }

        [Fact]
        public void Linq_SelectMany_WithComplexProjection()
        {
            var result = from name in Result<string, string>.Ok("Alice")
                         from age in Result<int, string>.Ok(30)
                         select $"{name} is {age} years old";
            Assert.Equal("Alice is 30 years old", result.Value);
        }

        // ========== Chaining / Realistic Scenarios ==========

        [Fact]
        public void Chain_MapBindMatch_OkPath()
        {
            var final = Result<int, string>.Ok(4)
                .Map(x => x * 2)           // 8
                .Bind(x => x > 10 ? Result<int, string>.Error("too large") : Result<int, string>.Ok(x))
                .Match(
                    ok: x => $"Result: {x}",
                    error: e => $"Error: {e}"
                );
            Assert.Equal("Result: 8", final);
        }

        [Fact]
        public void Chain_MapBindMatch_ErrorPath()
        {
            var final = Result<int, string>.Ok(6)
                .Map(x => x * 2)           // 12
                .Bind(x => x > 10 ? Result<int, string>.Error("too large") : Result<int, string>.Ok(x))
                .Match(
                    ok: x => $"Result: {x}",
                    error: e => $"Error: {e}"
                );
            Assert.Equal("Error: too large", final);
        }

        [Fact]
        public void ValueOr_AfterChain()
        {
            var value = Result<int, string>.Error("missing")
                .Map(x => x * 2)
                .ValueOr(99);
            Assert.Equal(99, value);
        }

        // ========== Value Types & Nullability ==========

        [Fact]
        public void Ok_WithValueTypeDefault_IsValid()
        {
            var result = Result<int, string>.Ok(0);
            Assert.True(result.IsOk);
            Assert.Equal(0, result.Value);
        }

        [Fact]
        public void Error_WithReferenceTypeError_CanHoldNull()
        {
            var result = Result<int, string>.Error(null!);
            Assert.True(result.IsError);
            Assert.Null(result.ErrorValue);
        }

        [Fact]
        public void MapError_WithDifferentErrorType()
        {
            var result = Result<int, string>.Error("42");
            var mapped = result.MapError(err => int.Parse(err));
            Assert.True(mapped.IsError);
            Assert.Equal(42, mapped.ErrorValue);
        }

        [Fact]
        public void Bind_ChangingErrorType()
        {
            var result = Result<int, string>.Ok(10);
            var bound = result.Bind(x => x > 5
                ? Result<int, string>.Ok(x)
                : Result<int, string>.Error("value too small"));
            Assert.True(bound.IsOk);
            Assert.Equal(10, bound.Value);
        }
    }
}
