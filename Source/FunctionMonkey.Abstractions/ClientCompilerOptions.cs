using System;

namespace FunctionMonkey.Model
{
    public class ClientCompilerOptions
    {
        public string OutputFolder { get; set; }
        
        public string Namespace { get; set; }
        
        public string AssemblyName { get; set; }
        
        public bool OutputNuget { get; set; }
        
        public Type VersioningStrategy { get; set; }
    }
}