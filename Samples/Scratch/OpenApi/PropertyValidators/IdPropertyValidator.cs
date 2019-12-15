using FluentValidation;

namespace OpenApi.PropertyValidators
{
    public class IdPropertyValidator
    {
        public static void Rule<T>(IRuleBuilder<T, string> rule)
        {
            rule
                .Length(32);
        }
    }
}
