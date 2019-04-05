using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using OutputTriggerSample.Model;

namespace OutputTriggerSample.Commands
{
    public class SubmitInvoiceCommand : ICommand<InvoiceSummary>
    {
        public IReadOnlyCollection<InvoiceItem> Items { get; set; }
    }
}
