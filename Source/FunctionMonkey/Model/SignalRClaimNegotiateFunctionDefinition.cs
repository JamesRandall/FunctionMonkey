using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Model
{
    public class SignalRClaimTypeNegotiateCommand { }

    public class SignalRClaimNegotiateFunctionDefinition : HttpFunctionDefinition
    {
        public SignalRClaimNegotiateFunctionDefinition() : base(typeof(SignalRClaimTypeNegotiateCommand))
        {

        }

        public string ConnectionStringSettingName { get; set; }

        public string HubName { get; set; }

        public string ClaimType { get; set; }
    }
}
