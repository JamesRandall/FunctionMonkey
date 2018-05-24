using System;
using System.Collections.Generic;

namespace FunctionMonkey.Abstractions.Contexts
{
    public class BlobContext
    {
        public string BlobTrigger { get; set; }

        public Uri Uri { get; set; }

        public IDictionary<string, string> Metadata { get; set; }

        // Need to decide what to do about this - I don't want to have to add a reference to storage into the 
        // abstractions but neither do I essentially want to clone the class.
        //public BlobProperties BlobProperties { get; set; }
    }
}
