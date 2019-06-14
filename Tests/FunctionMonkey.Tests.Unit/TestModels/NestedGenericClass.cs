namespace FunctionMonkey.Tests.Unit.TestModels
{
    internal class NestedGenericClassContainer
    {
        public class NestedGenericClass<TType>
        {
            public TType Value { get; set; }
        }
    }
}