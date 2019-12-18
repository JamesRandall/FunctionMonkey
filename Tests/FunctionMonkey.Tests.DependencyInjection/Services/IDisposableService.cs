namespace FunctionMonkey.Tests.DependencyInjection.Services
{
	using System;

	public interface IDisposableService : IDisposable
	{
		void Run();
	}
}
