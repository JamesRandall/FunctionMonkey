using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Model
{
    public class SignalRBindingExpressionNegotiateCommand { }

    public class SignalRBindingExpressionNegotiateFunctionDefinition : HttpFunctionDefinition
    {
        public SignalRBindingExpressionNegotiateFunctionDefinition() : base(typeof(SignalRBindingExpressionNegotiateCommand))
        {

        }

        public string ConnectionStringSettingName { get; set; }

        public string HubName { get; set; }

        public string UserIdExpression { get; set; }

        public bool HasUserIdExpression => !string.IsNullOrWhiteSpace(UserIdExpression);
    }
}
