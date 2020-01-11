using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    public class ClientCompilerOptionsBuilder : IClientCompilerOptionsBuilder
    {
        private readonly ClientCompilerOptions _options;

        public ClientCompilerOptionsBuilder(ClientCompilerOptions options)
        {
            _options = options;
        }
        
        public IClientCompilerOptionsBuilder OutputFolder(string folder)
        {
            throw new System.NotImplementedException();
        }

        public IClientCompilerOptionsBuilder Namespace(string ns)
        {
            throw new System.NotImplementedException();
        }

        public IClientCompilerOptionsBuilder AssemblyName(string assemblyName)
        {
            throw new System.NotImplementedException();
        }

        public IClientCompilerOptionsBuilder OutputNuget()
        {
            throw new System.NotImplementedException();
        }

        public IClientCompilerOptionsBuilder VersioningStrategy<TVersioningStrategy>()
        {
            throw new System.NotImplementedException();
        }
    }
}