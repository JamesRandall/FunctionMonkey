using System;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Compiler.Core;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    internal class CompilerOptionsBuilder : ICompilerOptionsBuilder
    {
        private readonly CompilerOptions _options;

        public CompilerOptionsBuilder(CompilerOptions options)
        {
            _options = options;
        }

        public ICompilerOptionsBuilder MediatorTypeSafetyEnforcer<TMediatorTypeSafetyEnforcer>() where TMediatorTypeSafetyEnforcer : IMediatorTypeSafetyEnforcer
        {
            _options.MediatorTypeSafetyEnforcer = typeof(TMediatorTypeSafetyEnforcer);
            return this;
        }

        public ICompilerOptionsBuilder MediatorResultTypeExtractor<TMediatorResultTypeExtractor>() where TMediatorResultTypeExtractor : IMediatorResultTypeExtractor
        {
            _options.MediatorResultTypeExtractor = typeof(TMediatorResultTypeExtractor);
            return this;
        }

        public ICompilerOptionsBuilder HttpTarget(CompileTargetEnum target)
        {
            _options.HttpTarget = target;
            return this;
        }

        public ICompilerOptionsBuilder OutputSourceTo(string folder)
        {
            _options.OutputSourceTo = folder;
            return this;
        }

        public ICompilerOptionsBuilder CreateClient(Action<IClientCompilerOptionsBuilder> builder)
        {
            if (_options.Client == null)
            {
                _options.Client = new ClientCompilerOptions();
            }

            builder(new ClientCompilerOptionsBuilder(_options.Client));
            return this;
        }
    }
}