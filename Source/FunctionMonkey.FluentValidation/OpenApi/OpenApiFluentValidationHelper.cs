using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;

namespace FunctionMonkey.FluentValidation.OpenApi
{
    public static class OpenApiFluentValidationHelper
    {
        /// <summary>
        /// Is supported swagger numeric type.
        /// </summary>
        public static bool IsNumeric(this object value) => value is int || value is long || value is float || value is double || value is decimal;

        /// <summary>
        /// Convert numeric to int.
        /// </summary>
        public static int NumericToInt(this object value) => Convert.ToInt32(value);

        /// <summary>
        /// Convert numeric to double.
        /// </summary>
        public static double NumericToDouble(this object value) => Convert.ToDouble(value);

        /// <summary>
        /// Convert numeric to decimal.
        /// </summary>
        public static decimal NumericToDecimal(this object value) => Convert.ToDecimal(value);

        /// <summary>
        /// Returns not null dictionary.
        /// </summary>
        public static Dictionary<TKey, TValue> NotNull<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            return dictionary ?? new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Returns not null enumeration.
        /// </summary>
        public static IEnumerable<TValue> NotNull<TValue>(this IEnumerable<TValue> collection)
        {
            return collection ?? Array.Empty<TValue>();
        }

        /// <summary>
        /// Returns validators by property name.
        /// </summary>
        /// <param name="validator">Validator</param>
        /// <param name="name">Property name.</param>
        /// <returns>enumeration or null.</returns>
        public static IEnumerable<IPropertyValidator> GetValidatorsForMember(this IValidator validator, string name)
        {
            return (validator as IEnumerable<IValidationRule>)
                .NotNull()
                .OfType<PropertyRule>()
                .Where(propertyRule => propertyRule.Condition == null && propertyRule.AsyncCondition == null && propertyRule.PropertyName?.Equals(name, StringComparison.InvariantCulture) == true)
                .SelectMany(propertyRule => propertyRule.Validators);
        }
    }
}
