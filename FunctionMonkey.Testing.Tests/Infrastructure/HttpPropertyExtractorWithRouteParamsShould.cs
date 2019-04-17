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
    public class RouteParamTestCommand : ICommand
    {
        public int A { get; set; }
        public int? B { get; set; }
        public int? C { get; set; }
    }

    public class HttpPropertyExtractorWithRouteParamsShould
    {
        private readonly HttpParameterExtractor _parameterExtractor;

        public HttpPropertyExtractorWithRouteParamsShould()
        {
            var definitions = new List<AbstractFunctionDefinition>();
            var routeConfiguration = new HttpRouteConfiguration();
            ConnectionStringSettingNames connStringSettingsNames = null;

            var functionBuilder = new HttpFunctionBuilder(connStringSettingsNames, routeConfiguration, definitions)
                .HttpFunction<RouteParamTestCommand>("/testcommand/{A}/{B}/{C?}");

            var httpFunctionDefinition = definitions.First() as HttpFunctionDefinition;

            _parameterExtractor = new HttpParameterExtractor(httpFunctionDefinition);
        }

        [Fact]
        public void ExtractPossibleRouteParametersAsNotOptional()
        {

            var httpParams = _parameterExtractor.ExtractRouteParameters();

            var param = httpParams.First(x => x.Name == "A");
            Assert.Equal(typeof(int), param.Type);
            Assert.Equal("System.Int32", param.TypeName);
            Assert.False(param.IsOptional);
            Assert.False(param.IsNullable);
            Assert.False(param.IsNullableType);
            Assert.False(param.IsString);
        }

        [Fact]
        public void ExtractPossibleRouteParametersAsNotOptional2()
        {

            var httpParams = _parameterExtractor.ExtractRouteParameters();

            var param = httpParams.First(x => x.Name == "B");
            Assert.Equal(typeof(int?), param.Type);
            Assert.Equal("System.Nullable<System.Int32>", param.TypeName);
            Assert.False(param.IsOptional);
            Assert.True(param.IsNullable);
            Assert.True(param.IsNullableType);
            Assert.False(param.IsString);
        }

        [Fact]
        public void ExtractPossibleRouteParametersAsOptional()
        {

            var httpParams = _parameterExtractor.ExtractRouteParameters();

            var param = httpParams.First(x => x.Name == "C");
            Assert.Equal(typeof(int?), param.Type);
            Assert.Equal("System.Nullable<System.Int32>", param.TypeName);
            Assert.True(param.IsOptional);
            Assert.True(param.IsNullable);
            Assert.True(param.IsNullableType);
            Assert.False(param.IsString);
        }
    }
}
