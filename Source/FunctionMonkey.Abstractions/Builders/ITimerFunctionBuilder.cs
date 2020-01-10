using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface ITimerFunctionBuilder
    {
        /// <summary>
        /// Creates a Timer function that fires each specified interval with a command created with its default constructor
        /// </summary>
        /// <typeparam name="TCommand">The type of command</typeparam>
        /// <param name="cronExpression">The periodicity of the function</param>
        /// <returns>A function builder</returns>
        ITimerFunctionOptionsBuilder<TCommand> Timer<TCommand>(string cronExpression);

        /// <summary>
        /// Creates a Timer function that fires each specified interval with a command created using the factory
        /// </summary>
        /// <typeparam name="TCommand">The type of command</typeparam>
        /// <typeparam name="TTimerCommandFactoryType">Factory implementation for the command</typeparam>
        /// <param name="cronExpression">The periodicity of the function</param>
        /// <returns>A function builder</returns>
        ITimerFunctionOptionsBuilder<TCommand> Timer<TCommand, TTimerCommandFactoryType>(string cronExpression)
            where TTimerCommandFactoryType : ITimerCommandFactory<TCommand>;
    }
}
