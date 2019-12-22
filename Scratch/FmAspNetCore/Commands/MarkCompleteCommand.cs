using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FmAspNetCore.Commands
{
    public class MarkCompleteCommand : ICommand
    {
        public Guid Id { get; set; }
    }
}