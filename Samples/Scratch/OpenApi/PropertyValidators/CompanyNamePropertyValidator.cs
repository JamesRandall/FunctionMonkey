using FluentValidation;

namespace OpenApi.PropertyValidators
{
    public class CompanyNamePropertyValidator
    {
        public static void Rule<T>(IRuleBuilder<T, string> rule)
        {
            rule
                .MaximumLength(70);
        }
    }
}
