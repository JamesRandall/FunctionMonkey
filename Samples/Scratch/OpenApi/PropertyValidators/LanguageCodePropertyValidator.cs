using FluentValidation;

namespace OpenApi.PropertyValidators
{
    public class LanguageCodePropertyValidator
    {
        public static void Rule<T>(IRuleBuilder<T, string> rule)
        {
            rule
                .Length(2)
                .Matches(@"(?i)^(en|de|fr)$");
        }
    }
}
