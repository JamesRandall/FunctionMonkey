using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.SignalR
{
    public class NegotiateCommand : ICommand<SignalRNegotiateResponse>
    {
        // For anyone using this as a reference:
        // You wouldn't normally around a user ID to be passed in on a query parameter as this allows
        // But its to support integration tests that want to pretend to be different made up users
        public string UserId { get; set; }
    }
}
