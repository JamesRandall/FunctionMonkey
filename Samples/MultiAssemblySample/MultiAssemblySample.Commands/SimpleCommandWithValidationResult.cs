using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Commanding.Abstractions.Validation;

namespace MultiAssemblySample.Commands
{
    public class SimpleCommandWithValidationResult : ICommand<ValidationResult<string>>
    {
        public int SomeInputValue { get; set; }
    }
}
