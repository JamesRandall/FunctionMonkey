using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using OutputTriggerSample.Commands;
using OutputTriggerSample.Model;

namespace OutputTriggerSample.Handlers
{
    class SubmitInvoiceCommandHandler : ICommandHandler<SubmitInvoiceCommand, InvoiceSummary>
    {
        public Task<InvoiceSummary> ExecuteAsync(SubmitInvoiceCommand command, InvoiceSummary previousResult)
        {
            return Task.FromResult(new InvoiceSummary
            {
                Id = Guid.NewGuid(),
                TotalValue = command.Items.Sum(x => x.Quantity * x.UnitPrice)
            });
        }
    }
}
