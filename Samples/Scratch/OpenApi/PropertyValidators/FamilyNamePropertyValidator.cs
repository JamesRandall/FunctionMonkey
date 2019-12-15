using FluentValidation;

namespace OpenApi.PropertyValidators
{
    public class FamilyNamePropertyValidator
    {
        public static void Rule<T>(IRuleBuilder<T, string> rule)
        {
            rule
                .MaximumLength(35);
        }
    }
}
