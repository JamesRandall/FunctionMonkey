using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace ServiceBusSample.Commands
{
    public class NewUserRegistrationCommand : ICommand
    {
        public Guid UserId { get; set; }

        public string Username { get; set; }

        public string EmailAddress { get; set; }
    }
}
