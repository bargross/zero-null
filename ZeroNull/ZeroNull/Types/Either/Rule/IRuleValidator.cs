namespace ZeroNull.Types.Either.Rule
{
    internal interface IRuleValidator<TLeft, TRight> : IDisposable
    {
        bool TerminateOnFail { get; set; }
        bool IsLeftValue { get; set; }
        int FailedCount { get; }
        int RuleCount { get; }

        IRuleValidator<TLeft, TRight> AddRule(string ruleName, Func<TLeft, bool> rule);
        IRuleValidator<TLeft, TRight> AddRule(string ruleName, Func<TRight, bool> rule);

        IRuleValidator<TLeft, TRight> Replace(string ruleName, Func<TLeft, bool> replacement);
        IRuleValidator<TLeft, TRight> Replace(string ruleName, Func<TRight, bool> replacement);

        bool ValidateRuleFor(TLeft left);
        bool ValidateRuleFor(TRight right);
        void ResetRulesForLeftValue();
        void ResetRulesForRightValue();
        bool GetRuleValidationResult(string ruleName);
    }
}