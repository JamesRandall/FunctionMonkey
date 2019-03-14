using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace FunctionMonkey.Tests.Integration.Http.Helpers
{
    // This is a replication of the classes in FunctionMonkey.Abstractions - they are replicated
    // rather than included via package, reference, or source link to allow the integration tests
    // to be 100% independent of Function Monkey itself - this makes it a lot easier to use the
    // integration tests in dev / debug scenarios.
    public class ValidationResult
    {
        public IReadOnlyCollection<ValidationError> Errors { get; set; } = new ValidationError[0];

        public bool IsValid => Errors == null || Errors.Count == 0;

        // Validate our precanned validation state
        public void ValidateResponse()
        {
            Assert.False(IsValid);
            Assert.Equal(1, Errors.Count);
            ValidationError error = Errors.Single();
            Assert.Equal(SeverityEnum.Error, error.Severity);
            Assert.Equal("NotEqualValidator", error.ErrorCode);
            Assert.Equal("Value", error.Property);
            Assert.Equal("'Value' must not be equal to '0'.", error.Message);
        }
    }
}
