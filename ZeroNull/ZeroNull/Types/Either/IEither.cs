using ZeroNull.Types.Either.Rule;

namespace ZeroNull.Types.Either
{
    public interface IEither<TLeft, TRight>: IDisposable
    {
        public bool IsValid { get; }
        bool IsLeftValid { get; }
        bool IsRightValid { get; }
        
        T GetValue<T>();
        void ResetRulesForLeftValue();
        void ResetRulesForRightValue();
        void ResetRules();
        bool GetValidationResultForRule(string ruleName);
        //void SetValidatorOptions(Action<IRuleValidator<TLeft, TRight>> validator);
    }
}
