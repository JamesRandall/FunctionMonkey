using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Builders;
using FunctionMonkey.Infrastructure;
using FunctionMonkey.Model;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FunctionMonkey.Testing.Tests.Infrastructure
{
    public class TestCommand : ICommand
    {
        public int IntRequired { get; set; }
        public int? IntOptional { get; set; }
    }

    public class HttpPropertyExtractorWithQueryParamsShould
    {
        private readonly HttpParameterExtractor _parameterExtractor;

        public HttpPropertyExtractorWithQueryParamsShould()
        {
            var definitions = new List<AbstractFunctionDefinition>();
            var routeConfiguration = new HttpRouteConfiguration();
            ConnectionStringSettingNames connStringSettingsNames = null;

            var functionBuilder = new HttpFunctionBuilder(connStringSettingsNames, routeConfiguration, definitions)
                .HttpFunction<TestCommand>("/testcommand");

            var httpFunctionDefinition = definitions.First() as HttpFunctionDefinition;
            httpFunctionDefinition.RouteParameters = new HttpParameter[0];

            _parameterExtractor = new HttpParameterExtractor(httpFunctionDefinition);
        }

        [Fact]
        public void ExtractPossibleQueryParametersAsOptional()
        {

            var httpParams = _parameterExtractor.ExtractPossibleQueryParameters();

            var param = httpParams.First(x => x.Name == "IntRequired");
            Assert.Equal(typeof(int), param.Type);
            Assert.Equal("System.Int32", param.TypeName);
            Assert.False(param.IsOptional);
            Assert.False(param.IsNullable);
            Assert.False(param.IsNullableType);
            Assert.False(param.IsString);
        }

        [Fact]
        public void ExtractPossibleQueryParametersAsOptional2()
        {

            var httpParams = _parameterExtractor.ExtractPossibleQueryParameters();

            var param = httpParams.First(x => x.Name == "IntOptional");
            Assert.Equal(typeof(int?), param.Type);
            Assert.Equal("System.Nullable<System.Int32>", param.TypeName);
            Assert.True(param.IsOptional);
            Assert.True(param.IsNullable);
            Assert.True(param.IsNullableType);
            Assert.False(param.IsString);
        }
    }
}
