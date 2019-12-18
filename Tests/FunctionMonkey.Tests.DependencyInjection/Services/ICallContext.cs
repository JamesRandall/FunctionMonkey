namespace FunctionMonkey.Tests.DependencyInjection.Services
{
	public interface ICallContext
	{
		string Name { get; }
		string Subject { get; }
	}
}
