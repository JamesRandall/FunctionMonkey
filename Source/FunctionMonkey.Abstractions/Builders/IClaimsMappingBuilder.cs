using System;
using System.Linq.Expressions;

namespace FunctionMonkey.Abstractions.Builders
{
    /// <summary>
    /// Provides an interface that allows claims to be mapped to commands
    /// </summary>
    public interface IClaimsMappingBuilder
    {
        /// <summary>
        /// Will map a claim of the given type to any property with the name propertyName.
        /// This is useful if you take a consistent approach to naming for example
        /// </summary>
        /// <param name="claimType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        IClaimsMappingBuilder MapClaimToPropertyName(string claimType, string propertyName);

        /// <summary>
        /// Maps a claim to a property on a specific command type. These mappings will take precedence over the geneic claim name
        /// to property name mappings
        /// </summary>
        /// <typeparam name="TCommand">The type of command</typeparam>
        /// <param name="claimType">The claim type</param>
        /// <param name="getProperty">The property of the command to map to</param>
        /// <returns></returns>
        IClaimsMappingBuilder MapClaimToCommandProperty<TCommand>(string claimType, Expression<Func<TCommand, object>> getProperty);
    }
}
