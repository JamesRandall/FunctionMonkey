using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Builders;

namespace FunctionMonkey.Infrastructure
{
    internal static class DefaultMediatorSettings
    {
        public static void SetDefaultsIfRequired(FunctionHostBuilder builder)
        {
            if (builder.Options.MediatorResultTypeExtractor == null)
            {
                builder.Options.MediatorResultTypeExtractor = typeof(DefaultMediatorResultTypeExtractor);
            }

            if (builder.Options.MediatorTypeSafetyEnforcer == null)
            {
                builder.Options.MediatorTypeSafetyEnforcer = typeof(DefaultMediatorTypeSafetyEnforcer);
            }

            if (builder.MediatorType == null)
            {
                builder.MediatorType = typeof(DefaultMediatorDecorator);
            }
        }
    }
}