namespace FunctionMonkey.Model.OutputBindings
{
    public class CosmosOutputBinding : AbstractConnectionStringOutputBinding
    {
        public bool IsCollection { get; set; }
        
        public string DatabaseName { get; set; }
        
        public string CollectionName { get; set; }
        
        public CosmosOutputBinding(string commandResultTypeName, string connectionStringSettingName) : base(commandResultTypeName, connectionStringSettingName)
        {
        }
    }
}