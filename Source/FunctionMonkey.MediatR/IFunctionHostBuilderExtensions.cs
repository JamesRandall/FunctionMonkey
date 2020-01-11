using FunctionMonkey.Abstractions.Builders;

namespace FunctionMonkey.MediatR
{
    public static class IFunctionHostBuilderExtensions
    {
        public static IFunctionHostBuilder UseMediatR(this IFunctionHostBuilder builder)
        {
            builder
                .CompilerOptions(options =>options
                    .MediatorTypeSafetyEnforcer<MediatRTypeSafetyEnforcer>()
                    .MediatorResultTypeExtractor<MediatRResultTypeExtractor>()
                )
                .Mediator<MediatRDecorator>();
            return builder;
        }
    }
}