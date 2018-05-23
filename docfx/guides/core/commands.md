# Commands

Commands are serializable C# classes that must implement one of the below interfaces:

|Interface|Description|
|---------|-----------|
|ICommand|Declares that the class is a command that expects no result to be returned|
|ICommand&lt;TResult&gt;|Declares that the class is a command that expects a result of type TResult to be returned|

Neither interface require any methods or properties to be implemented and are simply used to identify the commands within the framework and to provide type safety where a result is required.

Additionally a command may optionally implement the _IIdentifiableCommand_ interface if it is able to provide a universally unique ID (i.e. an ID that is unique within the entire scope and lifetime of your application and it's data stores - for example a UUID). If implemented this ID will be used in audit operations.

## Examples of Commands

### ICommand

    public class RegisterUserCommand : ICommand
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }

### ICommand<TResult>

    public class GetUserQuery : ICommand<User>
    {
        public Guid UserId { get; set; }
    }

### IIdentifiableCommand

    public class UpdateUserCommand : ICommand<ValidationResponse>, IIdentifiableCommand
    {
        public string CommandId { get; set; } // the get is required by IIdentifiableCommand
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
