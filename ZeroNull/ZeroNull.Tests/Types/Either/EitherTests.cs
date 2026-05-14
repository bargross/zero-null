using ZeroNull.Types.Either;

namespace ZeroNull.Tests.Types.Either
{
    public class EitherTests
    {
        private Either<string, int> _either;

        public EitherTests()
        {
            _either = new Either<string, int>();
        }

        [Fact(Skip = "Invalid test as the visibility of rule validator is internal only, disabled until i change strategy")]
        public void SetValidatorOptions_RuleNameNotProvidedForLeft_ThrowsArgumentExcption()
        {
            Assert.Throws<ArgumentException>(() => {
                //_either.SetValidatorOptions(options => options.AddRule(null, value => !string.IsNullOrWhiteSpace(value)) );
            });
        }

        [Fact(Skip = "Invalid test as the visibility of rule validator is internal only, disabled until i change strategy")]
        public void SetValidatorOptions_RuleNameNotProvidedForRight_ThrowsArgumentExcption()
        {
            Assert.Throws<ArgumentException>(() => {
                //_either.SetValidatorOptions(options => options.AddRule(null, value => value > 0 && value < 10));
            });
        }

        [Fact]
        public void ContainsRule_RuleNameNotProvided_ReturnsFalse() => Assert.False(_either.ContainsRule("A"));

        [Fact(Skip = "Invalid test as the visibility of rule validator is internal only, disabled until i change strategy")]
        public void ContainsRule_RuleNameProvided_ReturnsTrue()
        {
            var ruleName = "A";
            //_either.SetValidatorOptions( options => options.AddRule(ruleName, value => value == ruleName));

            var result = _either.ContainsRule(ruleName);

            Assert.True(result);
        }

        [Fact(Skip = "Invalid test as the visibility of rule validator is internal only, disabled until i change strategy")]
        public void SetValidatorOptions_RuleProvidedForLeft_ValidatorContainsRule()
        {
            var ruleName = "B";

            //_either.SetValidatorOptions( options => {
            //    options.AddRule(ruleName, value => !string.IsNullOrWhiteSpace(value));
            //});

            Assert.True(_either.ContainsRule(ruleName));
        }

        [Fact(Skip = "Invalid test as the visibility of rule validator is internal only, disabled until i change strategy")]
        public void SetValidatorOptions_RuleProvidedForRight_ValidatorContainsRule()
        {
            var ruleName = "B";

            //_either.SetValidatorOptions(options => {
            //    options.AddRule(ruleName, value => value > 0 && value < 10);
            //});

            Assert.True(_either.ContainsRule(ruleName));
        }

        [Fact]
        public void GetValue_RuleProvidedForLeftWithValueOutsideOfBounds_ThrowsInvalidCastException()
        {
            _either = " ";

            Assert.Throws<InvalidCastException>(() => _either.GetValue<int>());
        }

        [Fact]
        public void GetValue_RuleProvidedForRightWithValueOutsideOfBounds_ThrowsInvalidCastException()
        {
            _either = 11;

            Assert.Throws<InvalidCastException>(() => _either.GetValue<string>());
        }

        [Fact(Skip = "Invalid test as the visibility of rule validator is internal only, disabled until i change strategy")]
        public void GetValue_RuleProvidedForLeftWithValueOutsideOfBounds_ThrowsRuleValidationException()
        {
            var invalidValue = " ";
            var ruleName = "A";

            //_either.SetValidatorOptions(options => {
            //    options.TerminateOnFail = true;
            //    options.AddRule(ruleName, value => !string.IsNullOrWhiteSpace(value));
            //});

            //_either = invalidValue;

            //Assert.Throws<RuleValidationException>(() => _either.GetValue<string>());
            Assert.False(_either.GetValidationResultForRule(ruleName));
            Assert.False(_either.IsValid);
        }

        [Fact(Skip = "Invalid test as the visibility of rule validator is internal only, disabled until i change strategy")]
        public void GetValue_RuleProvidedForRightWithValueOutsideOfBounds_ThrowsRuleValidationException()
        {
            var invalidValue = 11;
            var ruleName = "A";

            //_either.SetValidatorOptions(options => {
            //    options.TerminateOnFail = true;
            //    options.AddRule(ruleName, value => value >= 0 && value <= 10);
            //});

            //_either = invalidValue;

            //Assert.Throws<RuleValidationException>(() => _either.GetValue<int>());
            Assert.False(_either.GetValidationResultForRule(ruleName));
            Assert.False(_either.IsValid);
        }

        [Fact(Skip = "Invalid test as the visibility of rule validator is internal only, disabled until i change strategy")]
        public void GetValue_RuleProvidedForLeftWithValueBetweenOfBounds_ReturnsLeftValue()
        {
            var expected = "Valid String";
            var ruleName = "A";

            //_either.SetValidatorOptions(options => {
            //    options.TerminateOnFail = true;
            //    options.AddRule(ruleName, value => !string.IsNullOrWhiteSpace(value));
            //});

            _either = expected;

            Assert.True(_either.GetValidationResultForRule(ruleName));
            Assert.True(_either.IsValid);
            Assert.True(_either.ContainsRule(ruleName));
            Assert.Equal(expected, _either.GetValue<string>());
        }

        [Fact(Skip = "Invalid test as the visibility of rule validator is internal only, disabled until i change strategy")]
        public void GetValue_RuleProvidedForRightWithValueBetweenOfBounds_ReturnsRightValue()
        {
            var expected = 10;
            var ruleName = "A";

            //_either.SetValidatorOptions(options => {
            //    options.TerminateOnFail = true;
            //    options.AddRule(ruleName, value => value >= 0 && value <= 10);
            //});

            _either = expected;

            Assert.True(_either.GetValidationResultForRule(ruleName));
            Assert.True(_either.IsValid);
            Assert.True(_either.ContainsRule(ruleName));
            Assert.Equal(expected, _either.GetValue<int>());
        }

        [Fact]
        public void ImplicitOperatorTLeft_ValidLeftValueProvided_CastsEitherToTLeft()
        {
            var expected = "bla";
            _either = expected;

            var result = (string)_either;

            Assert.True(_either.IsPresent);
            Assert.True(_either.IsValid);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ImplicitOperatorTRight_ValidRightValueProvided_CastsEitherToTRight()
        {
            var expected = 123123;
            _either = expected;

            var result = (int)_either;

            Assert.True(_either.IsPresent);
            Assert.True(_either.IsValid);
            Assert.Equal(expected, result);
        }

        [Fact(Skip = "Invalid test as the visibility of rule validator is internal only, disabled until i change strategy")]
        public void ReplaceRule_NewRuleGivenToReplaceOldRule_ReplacesRuleForRight()
        {
            var ruleName = "C";

            //_either.SetValidatorOptions(options => options.AddRule(ruleName, value => value == ruleName));

            _either.ReplaceRule(ruleName, value => value == 1);
            _either = 1;

            Assert.True(_either.ContainsRule(ruleName));
            Assert.True(_either.IsValid);
        }

        [Fact(Skip = "Invalid test as the visibility of rule validator is internal only, disabled until i change strategy")]
        public void ReplaceRule_NewRuleGivenToReplaceOldRule_ReplacesRuleForLeft()
        {
            var ruleName = "C";

            //_either.SetValidatorOptions(options => options.AddRule(ruleName, value => value == 1));

            _either.ReplaceRule(ruleName, value => value == ruleName);
            _either = ruleName;

            Assert.True(_either.ContainsRule(ruleName));
            Assert.True(_either.IsValid);
        }

        [Fact(Skip = "Invalid test as the visibility of rule validator is internal only, disabled until i change strategy")]
        public void ResetRules_RulesProvidedForLeftAndRight_RulesAreCleared()
        {
            var ruleNameLeft = "A";
            var ruleNameRight = "B";

            //_either.SetValidatorOptions(options => {
            //    options.AddRule(ruleNameLeft, value => value == ruleNameRight)
            //    .AddRule(ruleNameRight, value => value > 1);
            //});

            var leftRuleBeforeClearing = _either.ContainsRule(ruleNameLeft);
            var rightRuleBeforeClearing = _either.ContainsRule(ruleNameRight);

            _either.ResetRules();

            var resultForLeft = _either.ContainsRule(ruleNameLeft);
            var resultForRight = _either.ContainsRule(ruleNameRight);

            Assert.True(leftRuleBeforeClearing);
            Assert.True(rightRuleBeforeClearing);
            Assert.False(resultForLeft);
            Assert.False(resultForRight);
        }
    }
}
