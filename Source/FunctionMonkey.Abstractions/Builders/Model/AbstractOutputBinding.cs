namespace FunctionMonkey.Abstractions.Builders.Model
{
    public abstract class AbstractOutputBinding
    {
        private readonly string _commandResultTypeItemName;
        private readonly AbstractFunctionDefinition _associatedFunctionDefinition;
        
        
        protected AbstractOutputBinding(AbstractFunctionDefinition associatedFunctionDefinition)
        {
            _associatedFunctionDefinition = associatedFunctionDefinition;
        }

        protected AbstractOutputBinding(string commandResultTypeItemName)
        {
            _commandResultTypeItemName = commandResultTypeItemName;
        }

        public string CommandResultItemTypeName => _associatedFunctionDefinition.CommandResultItemTypeName ?? _commandResultTypeItemName;
    }
}