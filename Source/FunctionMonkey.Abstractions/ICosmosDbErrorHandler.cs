using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace FunctionMonkey.Commanding.Cosmos.Abstractions
{
    /// <summary>
    /// Implementations of this handle Cosmos errors at the Function level.
    /// </summary>
    public interface ICosmosDbErrorHandler
    {
        /// <summary>
        /// Handle a error processing a Cosmos document.
        /// To abort processing of the current batch return false.
        /// </summary>
        /// <param name="ex">The exception that caused the error</param>
        /// <param name="document">The document that caused the error</param>
        /// <returns>True to continue processing, false to stop processing of the batch</returns>
        Task<bool> HandleError(Exception ex, Document document);
    }
}
