using System;
using FunctionMonkey.Abstractions.Extensions;
using FunctionMonkey.Tests.Unit.TestModels;
using Xunit;

namespace FunctionMonkey.Tests.Unit.Abstractions.Extensions
{
    public class TypeExtensionsShould
    {
        [Theory]
        [InlineData(
            typeof(SimpleClass), 
            "FunctionMonkey.Tests.Unit.TestModels.SimpleClass"
            )]
        [InlineData(
            typeof(EncapsulatingClass.EncapsulatedEnum), 
            "FunctionMonkey.Tests.Unit.TestModels.EncapsulatingClass.EncapsulatedEnum"
            )]
        [InlineData(
            typeof(SimpleGenericClass<string>), 
            "FunctionMonkey.Tests.Unit.TestModels.SimpleGenericClass<System.String>"
            )]
        [InlineData(
            typeof(NestedGenericClassContainer.NestedGenericClass<string>),
            "FunctionMonkey.Tests.Unit.TestModels.NestedGenericClassContainer.NestedGenericClass<System.String>"
            )]
        public void ReturnsExpectedTypeAsString(Type candidateType, string expected)
        {
            string result = candidateType.EvaluateType();
            Assert.Equal(expected, result);
        }
    }
}