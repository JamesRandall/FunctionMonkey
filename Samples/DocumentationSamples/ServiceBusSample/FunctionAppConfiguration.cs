using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using ServiceBusSample.Commands;
using ServiceBusSample.Handlers;

namespace ServiceBusSample
{
    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        private const string QueueName = "sendEmailQueue";
        private const string TopicName = "newuserregistration";
        private const string SubscriptionName = "telemetrysubscription";
        private const string ServiceBusConnectionName = "serviceBusConnection";

        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                    commandRegistry
                        .Register<SendEmailCommandHandler>()
                        .Register<NewUserRegistrationCommandHandler>()
                )
                .Functions(functions => functions
                    .ServiceBus(ServiceBusConnectionName, serviceBus => serviceBus
                        .QueueFunction<SendEmailCommand>(QueueName)
                        .SubscriptionFunction<NewUserRegistrationCommand>(TopicName, SubscriptionName)
                    )
                );
        }
    }
}
