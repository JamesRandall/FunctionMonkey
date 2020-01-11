namespace FunctionMonkey.Abstractions.Builders
{
    public interface IClientCompilerOptionsBuilder
    {
        IClientCompilerOptionsBuilder OutputFolder(string folder);

        IClientCompilerOptionsBuilder Namespace(string ns);

        IClientCompilerOptionsBuilder AssemblyName(string assemblyName);

        IClientCompilerOptionsBuilder OutputNuget();

        IClientCompilerOptionsBuilder VersioningStrategy<TVersioningStrategy>();
    }
}