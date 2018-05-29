using System;

namespace MultiAssemblySample.Commands.Abstract
{
    // This is a little artifical but it helps us validate that the compiler understands inheritance references
    public abstract class AbstractCommand
    {
        public string Message { get; set; }
    }
}
