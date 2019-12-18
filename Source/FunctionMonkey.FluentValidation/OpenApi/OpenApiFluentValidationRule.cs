using FluentValidation.Validators;
using System;

namespace FunctionMonkey.FluentValidation.OpenApi
{
    public class OpenApiFluentValidationRule
    {
        /// <summary>
        /// Rule name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Predicate to match property validator.
        /// </summary>
        public Func<IPropertyValidator, bool> Matches { get; set; }

        /// <summary>
        /// Modify Swagger schema action.
        /// </summary>
        public Action<OpenApiFluentValidationRuleContext> Apply { get; set; }

        /// <summary>
        /// Creates new instance of <see cref="OpenApiFluentValidationRule"/>.
        /// </summary>
        /// <param name="name">Rule name.</param>
        public OpenApiFluentValidationRule(string name)
        {
            Name = name;
        }
    }
}
