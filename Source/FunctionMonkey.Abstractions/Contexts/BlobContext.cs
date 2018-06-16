using System;
using System.Collections.Generic;

namespace FunctionMonkey.Abstractions.Contexts
{
    /// <summary>
    /// Blob trigger context information
    /// </summary>
    public class BlobContext
    {
        /// <summary>
        // The blog trigger
        /// </summary>
        public string BlobTrigger { get; set; }

        /// <summary>
        /// The URI of the blob
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Metadata associated with the blob
        /// </summary>
        public IDictionary<string, string> Metadata { get; set; }

        // Need to decide what to do about this - I don't want to have to add a reference to storage into the 
        // abstractions but neither do I essentially want to clone the class.
        //public BlobProperties BlobProperties { get; set; }
    }
}
