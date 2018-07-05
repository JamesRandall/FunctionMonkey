using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Extensions;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    class TimerFunctionBuilder : ITimerFunctionBuilder
    {
        private readonly IFunctionBuilder _functionBuilder;
        private readonly List<AbstractFunctionDefinition> _functionDefinitions;

        public TimerFunctionBuilder(IFunctionBuilder functionBuilder,
            List<AbstractFunctionDefinition> functionDefinitions)
        {
            _functionBuilder = functionBuilder;
            _functionDefinitions = functionDefinitions;
        }

        
        public IFunctionBuilder Timer<TCommand>(string cronExpression) where TCommand : ICommand
        {
            _functionDefinitions.Add(new TimerFunctionDefinition(typeof(TCommand))
            {
                CronExpression = cronExpression
            });
            return _functionBuilder;
        }

        public IFunctionBuilder Timer<TCommand, TTimerCommandFactoryType>(string cronExpression)
            where TCommand : ICommand
            where TTimerCommandFactoryType : ITimerCommandFactory<TCommand>
        {
            _functionDefinitions.Add(new TimerFunctionDefinition(typeof(TCommand))
            {
                CronExpression = cronExpression,
                TimerCommandFactoryType = typeof(TTimerCommandFactoryType),
                TimerCommandFactoryTypeName = typeof(TTimerCommandFactoryType).EvaluateType()
            });
            return _functionBuilder;
        }
    }
}
