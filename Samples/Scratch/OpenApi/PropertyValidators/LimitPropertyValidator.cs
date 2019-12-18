using FluentValidation;

namespace OpenApi.PropertyValidators
{
    public class LimitPropertyValidator
    {
        public static void Rule<T>(IRuleBuilder<T, int> rule)
        {
            rule
                .InclusiveBetween(1, 500);
        }

        public static void Rule<T>(IRuleBuilder<T, int?> rule)
        {
            rule
                .InclusiveBetween(1, 500);
        }
    }
}
