using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using FunctionMonkey.Tests.Integration.Commands;

namespace FunctionMonkey.Tests.Integration.Validators
{
    class BlobCommandValidator : AbstractValidator<BlobCommand>
    {
    }
}
