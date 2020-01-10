using System;

namespace FunctionMonkey.Abstractions
{
    /// <summary>
    /// Used by Function Monkey to determine if commands implement the appropriate interfaces for the in use mediator.
    /// This is only called by the compiler so there is no big need to have it be massively performant - refelction is
    /// fine!
    /// </summary>
    public interface IMediatorTypeSafetyEnforcer
    {
        bool IsValidType(Type commandType); 
        
        string Requirements { get; }
    }
}