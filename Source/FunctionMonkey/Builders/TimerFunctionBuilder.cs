using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Extensions;
using FunctionMonkey.Model;
using IFunctionBuilder = FunctionMonkey.Abstractions.Builders.IFunctionBuilder;

namespace FunctionMonkey.Builders
{
    class TimerFunctionBuilder<TCommandOuter> : ITimerFunctionBuilder
    {
        private readonly ConnectionStringSettingNames _connectionStringSettingNames;
        private readonly IFunctionBuilder _functionBuilder;
        private readonly List<AbstractFunctionDefinition> _functionDefinitions;

        public TimerFunctionBuilder(
            ConnectionStringSettingNames connectionStringSettingNames,
            IFunctionBuilder functionBuilder,
            List<AbstractFunctionDefinition> functionDefinitions)
        {
            _connectionStringSettingNames = connectionStringSettingNames;
            _functionBuilder = functionBuilder;
            _functionDefinitions = functionDefinitions;
        }

        
        public ITimerFunctionOptionsBuilder<TCommand> Timer<TCommand>(string cronExpression)
        {
            TimerFunctionDefinition timerFunctionDefinition = new TimerFunctionDefinition(typeof(TCommand))
            {
                CronExpression = cronExpression
            };

            _functionDefinitions.Add(timerFunctionDefinition);
            return new TimerFunctionOptionsBuilder<TCommand>(_connectionStringSettingNames, _functionBuilder, timerFunctionDefinition);
        }

        public ITimerFunctionOptionsBuilder<TCommand> Timer<TCommand, TTimerCommandFactoryType>(string cronExpression)
            where TTimerCommandFactoryType : ITimerCommandFactory<TCommand>
        {
            TimerFunctionDefinition timerFunctionDefinition = new TimerFunctionDefinition(typeof(TCommand))
            {
                CronExpression = cronExpression,
                TimerCommandFactoryType = typeof(TTimerCommandFactoryType),
                TimerCommandFactoryTypeName = typeof(TTimerCommandFactoryType).EvaluateType()
            };

            _functionDefinitions.Add(timerFunctionDefinition);
            return new TimerFunctionOptionsBuilder<TCommand>(_connectionStringSettingNames, _functionBuilder, timerFunctionDefinition);
        }


    }
}
