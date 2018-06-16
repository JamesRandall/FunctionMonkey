using System;

namespace FunctionMonkey
{
    /// <summary>
    /// In order for the .NET publishing process to not discard the root funcation assembly we need to couple it and
    /// our built assembly. We use this to stamp a class as a link back class to make a type reference between
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ReferenceLinkBackAttribute : Attribute
    {
    }
}
