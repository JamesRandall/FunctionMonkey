using System;
using System.Collections.Generic;
using System.Text;

namespace OutputTriggerSample.Model
{
    public class InvoiceItem
    {
        public string Code { get; set; }

        public double UnitPrice { get; set; }

        public int Quantity { get; set; }
    }
}
