using System;
using System.Collections.Generic;
using System.Text;
using FunctionMonkey.Infrastructure;
using Xunit;

namespace FunctionMonkey.Testing.Tests.Infrastructure
{
    public class NullableTestObject
    {
        public int A { get; set; }
        public int? B { get; set; }
    }

    public class HttpPropertyExtractorIsNullableShould
    {
        [Theory]
        [InlineData(typeof(int?), typeof(int))]
        [InlineData(typeof(float?), typeof(float))]
        [InlineData(typeof(bool?), typeof(bool))]
        public void IdentifyAsNullable(Type t, Type nullT)
        {
            Assert.Equal(nullT, Nullable.GetUnderlyingType(t));
            Assert.True(HttpParameterExtractor.IsNullableType(t));
        }

        [Theory]
        [InlineData(typeof(int))]
        [InlineData(typeof(float))]
        [InlineData(typeof(bool))]
        [InlineData(typeof(string))]
        [InlineData(typeof(object))]
        [InlineData(typeof(NullableTestObject))]
        public void IdentifyAsNotNullable(Type t)
        {
            Assert.Null(Nullable.GetUnderlyingType(t));
            Assert.False(HttpParameterExtractor.IsNullableType(t));
        }
    }
}
