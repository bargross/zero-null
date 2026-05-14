using ZeroNull.Types.Either.Rule;

namespace ZeroNull.Tests.Types.Rule
{
    public class RuleValidatorTests
    {
        private RuleValidator<int, string> _ruleValidator;

        public RuleValidatorTests()
        {
            _ruleValidator = new RuleValidator<int, string>();
        }

        [Fact]
        public void AddRule_RuleNameForLeftIsNull_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                _ruleValidator.AddRule(null, value => value > 0);
            });
        }

        [Fact]
        public void AddRule_RuleNameForRightIsNull_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                _ruleValidator.AddRule(null, value => !string.IsNullOrEmpty(value));
            });
        }

        [Fact]
        public void AddRule_RuleProvidedForLeft_RuleExists()
        {
            var ruleName = "greater than 0";
            _ruleValidator.AddRule(ruleName, value => value > 0);

            Assert.True(_ruleValidator.RuleCount == 1);
            Assert.True(_ruleValidator.ContainsRule(ruleName));
        }

        [Fact]
        public void AddRule_RuleProvidedForRight_RuleExists()
        {
            var ruleName = "not null or empty";
            _ruleValidator.AddRule(ruleName, value => !string.IsNullOrEmpty(value));

            Assert.True(_ruleValidator.RuleCount == 1);
            Assert.True(_ruleValidator.ContainsRule(ruleName));
        }

        [Fact]
        public void AddRule_RuleProvidedForLeftAndRight_RulesExists()
        {
            var ruleNameForRight = "not null or empty";
            var ruleNameForLeft = "greater than 0";

            _ruleValidator
                .AddRule(ruleNameForLeft, value => !string.IsNullOrEmpty(value))
                .AddRule(ruleNameForRight, value => value > 0);

            Assert.True(_ruleValidator.RuleCount == 2);
            Assert.True(_ruleValidator.ContainsRule(ruleNameForLeft));
            Assert.True(_ruleValidator.ContainsRule(ruleNameForRight));
        }
    }
}
