namespace FunctionMonkey.Abstractions.Builders.Model
{
    public abstract class AbstractOutputBinding
    {
        protected AbstractOutputBinding(string CommandResultItemTypeName)
        {
            this.CommandResultItemTypeName = CommandResultItemTypeName;
        }

        public string CommandResultItemTypeName { get; }
    }
}