using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionMonkey.Tests.Integration.Http.Helpers
{
    public enum SeverityEnum
    {
        /// <summary>
        /// An error
        /// </summary>
        Error,
        /// <summary>
        /// A warning
        /// </summary>
        Warning,
        /// <summary>
        /// Informational only
        /// </summary>
        Info
    }

    public class ValidationError
    {
        public SeverityEnum Severity { get; set; }

        public string ErrorCode { get; set; }

        public string Property { get; set; }

        public string Message { get; set; }
    }
}
