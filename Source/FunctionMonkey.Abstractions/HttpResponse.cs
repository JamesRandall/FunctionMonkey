using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionMonkey.Abstractions
{
    public class HttpResponse
    {
        public int StatusCode { get; set; }

        /// <summary>
        /// If not null this will be used for the response message, otherwise the default behaviour will be used:
        ///     Errors: "Error occurred"
        ///     Success: Command response body
        /// </summary>
        public byte[] Body { get; set; }
    }
}
