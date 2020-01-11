using System;

namespace FunctionMonkey.Abstractions
{
    public interface IMediatorResultTypeExtractor
    {
        Type CommandResultType(Type commandType);
    }
}