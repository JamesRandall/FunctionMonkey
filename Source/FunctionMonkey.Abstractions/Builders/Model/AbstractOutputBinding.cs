namespace FunctionMonkey.Abstractions.Builders.Model
{
    public abstract class AbstractOutputBinding
    {
        protected AbstractOutputBinding(string commandResultTypeName)
        {
            CommandResultTypeName = commandResultTypeName;
        }

        public string CommandResultTypeName { get; }
    }
}