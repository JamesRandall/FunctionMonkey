using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using FunctionMonkey.Abstractions.Validation;
using FunctionMonkey.Commanding.Abstractions.Validation;

namespace FunctionMonkey.Testing
{
    public class ValidatingDispatcher : ICommandDispatcher
    {
        private readonly ICommandDispatcher _underlyingDispatcher;
        private readonly IValidator _validator;

        public ValidatingDispatcher(ICommandDispatcher underlyingDispatcher, IValidator validator)
        {
            _underlyingDispatcher = underlyingDispatcher;
            _validator = validator;
        }

        public async Task<CommandResult<TResult>> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = new CancellationToken())
        {
            Validate(command);

            return await _underlyingDispatcher.DispatchAsync(command, cancellationToken);
        }

        public async Task<CommandResult> DispatchAsync(ICommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            Validate(command);
            return await _underlyingDispatcher.DispatchAsync(command, cancellationToken);
        }

        public ICommandExecuter AssociatedExecuter { get; } = null;

        private void Validate(ICommand command)
        {
            if (_validator == null)
            {
                return;
            }

            // The .Validate method uses its generic parameter to determine the type of the
            // command as it uses generics with the IServiceCollection, this has the unfortunate
            // side-effect of meaning you can't ask it to validate a none-concretely typed ICommand
            // This isn't an issue in Function Monkey itself as we have the specific command type
            // but does cause a problem here as if we call .Validate(command) command will be resolved
            // as being of type ICommand and so no validator will be found.
            //
            // To work round this we construct a method call using the concrete type of the command.
            // In test code this isn't performance sensitive so we don't bother compiling an expression
            // for it.
            MethodInfo methodInfo = _validator.GetType().GetMethod("Validate");
            MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(command.GetType());
            ValidationResult validationResult = (ValidationResult)genericMethodInfo.Invoke(_validator, new object[] {command});

            //ValidationResult validationResult = _validator.Validate(command);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult);
            }
        }
    }
}
