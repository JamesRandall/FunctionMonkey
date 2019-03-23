using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionMonkey.Commanding.Abstractions
{
    /// <summary>
    /// An optional interface that can be implemented by the return type of a command.
    /// It allows a blob name to be specified, Value will be serialized as JSON and written
    /// to the blob(s).
    /// </summary>
    public interface IBlobOutputCommandResult<TType>
    {
        string Name { get; set; }

        TType Value { get; set; }
    }
}
