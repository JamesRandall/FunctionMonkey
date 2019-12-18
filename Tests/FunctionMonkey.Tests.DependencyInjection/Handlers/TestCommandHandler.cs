namespace FunctionMonkey.Tests.DependencyInjection.Handlers
{
	using System.Threading.Tasks;

	using AzureFromTheTrenches.Commanding.Abstractions;

	using FunctionMonkey.Tests.DependencyInjection.Commands;
	using FunctionMonkey.Tests.DependencyInjection.Services;
	using Microsoft.Extensions.Logging;

	public class TestCommandHandler : ICommandHandler<TestCommand>
	{
		private readonly IDisposableService _service;
		private readonly ILogger _logger;

		public TestCommandHandler(IDisposableService service, ILogger logger)
		{
			_service = service;
			_logger = logger;
		}

		public Task ExecuteAsync(TestCommand command)
		{
			_logger.LogDebug("ExecuteAsync.");

			_service.Run();

			return Task.CompletedTask;
		}
	}
}
