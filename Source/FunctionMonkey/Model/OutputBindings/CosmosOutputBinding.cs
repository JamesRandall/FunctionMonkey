using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    public class CosmosOutputBinding : AbstractConnectionStringOutputBinding
    {
        public bool IsCollection { get; set; }
        
        public string DatabaseName { get; set; }
        
        public string CollectionName { get; set; }
        
        public CosmosOutputBinding(AbstractFunctionDefinition associatedFunctionDefinition, string connectionStringSettingName) : base(associatedFunctionDefinition, connectionStringSettingName)
        {
        }
        
        public CosmosOutputBinding(string commandResultType, string connectionStringSettingName) : base(commandResultType, connectionStringSettingName)
        {
        }
    }
}