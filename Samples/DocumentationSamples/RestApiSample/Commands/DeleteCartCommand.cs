using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace RestApiSample.Commands
{
    public class DeleteCartCommand : ICommand
    {
        [SecurityProperty]
        public Guid AuthenticatedUserId { get; set; }
    }
}
