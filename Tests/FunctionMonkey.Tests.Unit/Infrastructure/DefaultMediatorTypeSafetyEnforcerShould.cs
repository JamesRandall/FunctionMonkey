using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Infrastructure;
using Xunit;

namespace FunctionMonkey.Tests.Unit.Infrastructure
{
    public class DefaultMediatorTypeSafetyEnforcerShould
    {
        private class CommandWithNoResult : ICommand
        {
            
        }

        private class CommandWithResult : ICommand<string>
        {
            
        }

        private class DerivedFromCommandWithNoResult : CommandWithNoResult
        {
            
        }

        private class DerivedFromCommandWithResult : CommandWithResult
        {
            
        }

        private class InvalidCommandType
        {
            
        }

        [Theory]
        [InlineData(typeof(CommandWithResult))]
        [InlineData(typeof(CommandWithNoResult))]
        [InlineData(typeof(DerivedFromCommandWithNoResult))]
        [InlineData(typeof(DerivedFromCommandWithResult))]
        public void ReturnTrueForValidCommandTypes(Type candidateType)
        {
            DefaultMediatorTypeSafetyEnforcer testSubject = new DefaultMediatorTypeSafetyEnforcer();
            Assert.True(testSubject.IsValidType(candidateType));
        }

        [Fact]
        public void ReturnFalseForInvalidCommandType()
        {
            DefaultMediatorTypeSafetyEnforcer testSubject = new DefaultMediatorTypeSafetyEnforcer();
            Assert.False(testSubject.IsValidType(typeof(InvalidCommandType)));
        }
    }
}
