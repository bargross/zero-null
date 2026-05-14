namespace ZeroNull.Types.Either.Rule
{
    internal class RuleValidator<TLeft, TRight> : IRuleValidator<TLeft, TRight>
    {
        private IDictionary<string, (Func<TLeft, bool>, bool)> _rulesForLeft;
        private IDictionary<string, (Func<TRight, bool>, bool)> _rulesForRight;
        private bool _initialized;
        private bool _disposed = false;
        private int _capacity = 10;

        public IList<string> FailedValidationMessages { get; private set; }
        public bool TerminateOnFail { get; set; }
        public bool IsLeftValue { get; set; }
        public int FailedCount { get; private set; }
        public int RuleCount { get; private set; }

        public RuleValidator() => Init();
        
        // dispose destructor
        ~RuleValidator() => Dispose(false);
        
        public IRuleValidator<TLeft, TRight> AddRule(string ruleName, Func<TLeft, bool> rule)
        {
            AddRule(ruleName, rule, _rulesForLeft);
            return this;
        }

        public IRuleValidator<TLeft, TRight> AddRule(string ruleName, Func<TRight, bool> rule)
        {
            AddRule(ruleName, rule, _rulesForRight);
            return this;
        }

        public IRuleValidator<TLeft, TRight> Replace(string ruleName, Func<TLeft, bool> replacement)
        {
            _rulesForLeft[ruleName] = (replacement, false);
            return this;
        }
        public IRuleValidator<TLeft, TRight> Replace(string ruleName, Func<TRight, bool> replacement)
        {
            _rulesForRight[ruleName] = (replacement, false);
            return this;
        }

        public bool ValidateRuleFor(TLeft value) => ValidateRuleFor(value, _rulesForLeft);
        public bool ValidateRuleFor(TRight value) => ValidateRuleFor(value, _rulesForRight);

        public void ResetRulesForLeftValue() => _rulesForLeft.Clear();
        public void ResetRulesForRightValue() => _rulesForRight.Clear();

        public bool ContainsRule(string ruleName) => _rulesForLeft.ContainsKey(ruleName) || _rulesForRight.ContainsKey(ruleName);

        public bool GetRuleValidationResult(string ruleName)
        {
            if (_rulesForLeft.TryGetValue(ruleName, out var leftRuleDetails))
            {
                return leftRuleDetails.Item2;
            }

            if (_rulesForRight.TryGetValue(ruleName, out var rightRuleDetails))
            {
                return rightRuleDetails.Item2;
            }

            throw new KeyNotFoundException("Rule not found");
        }

        // Private Methods

        private void Init()
        {
            if (!_initialized)
            {
                _rulesForLeft = new Dictionary<string, (Func<TLeft, bool>, bool)>(_capacity);
                _rulesForRight = new Dictionary<string, (Func<TRight, bool>, bool)>(_capacity);
                FailedValidationMessages = new List<string>(_capacity);
                TerminateOnFail = false;

                _initialized = true;
            }
        }

        private void AddRule<T>(string ruleName, Func<T, bool> rule, IDictionary<string, (Func<T, bool>, bool)> ruleContainer) 
        {
            if(string.IsNullOrWhiteSpace(ruleName))
            {
                throw new ArgumentException("Rule must have a name");
            }

            RuleCount++;

            ruleContainer.Add(ruleName, (rule, false) );
        }

        private bool ValidateRuleFor<T>(T value, IDictionary<string, (Func<T, bool>, bool)> ruleContainer)
        {
            if(value == null)
            {
                throw new ArgumentException("Value is null");
            }

            if(ruleContainer.Count == 0)
            {
                return true;
            }

            for(int index = 0; index < ruleContainer.Count; ++index)
            {
                var ruleName = ruleContainer.Keys.ElementAt(index);
                var rule = ruleContainer[ruleName].Item1;

                if (TerminateOnFail && !rule.Invoke(value))
                {
                    FailedCount++;
                    return false;
                }

                if (!TerminateOnFail && !rule.Invoke(value))
                {
                    FailedValidationMessages.Add($"Value failed rule {ruleName} on validation");
                    FailedCount++;
                    continue;
                }

                ruleContainer[ruleName] = (ruleContainer[ruleName].Item1, true);
            }

            return !TerminateOnFail && FailedValidationMessages.Count > 0 || true;
        }

        // GC

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // managed resources

                    _rulesForLeft = null;
                    _rulesForRight = null;
                    FailedValidationMessages = null;
                }

                _disposed = true;
            }
        }
    }
}