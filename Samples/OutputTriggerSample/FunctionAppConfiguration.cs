using System.Net.Http;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using OutputTriggerSample.Commands;

namespace OutputTriggerSample
{
    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder.Functions(functions => functions
                .HttpRoute("/v1/invoice", route => route
                    .HttpFunction<SubmitInvoiceCommand>(HttpMethod.Post)
                    .OutputTo.ServiceBusQueue("serviceBusConnectionString", "invoiceSummaryQueue")
                )
            );
        }
    }
}
