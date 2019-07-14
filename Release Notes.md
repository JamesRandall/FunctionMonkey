# v3.0.5-beta2

* Corrected mistake in output binding for Session ID

# v3.0.4-beta1

This is released as a beta as it includes support for Service Bus session IDs which are currently only available in a beta version of the Microsoft.Azure.WebJobs.Extensions.ServiceBus package. Its not causing me any issues but the versioning of this as a beta is to give fair warning.

* Service bus functions (input and output) now have Session ID support
* Function options now include a NoCommandHandler method that allows an input to be mapped directly onto an output
* Addition of an ICommandWithNoHandler interface that allows the same to be used by implementing the interface at the command level
* Query parameters now work in conjuntion with a request body. Order of precedence is Body -> Query Param -> Header.
* Resolved issue #62 and added support for enum header bindings
* Fixed an issue with nested types (e.g. an enum declared inside a class, or a class declared inside a class) and header and query param binding
* Azure Functions will raise an Internal Server Error (rather than imo a bad request) when a route parameter value is supplied that doesn't match the target type, Function Monkey now accepts all route parameters as strings and internally converts to the correct type returning an appropriate Bad Request response if this cannot be done
* Query parameters with mismatched types now raise a bad request - issue #63
* Added support for HTTP query parameter arrays - issue #70
* ILogger injection no longer breaks when used inside an acceptance test - issue #69
* Added support for the function code to be documented as part of open API - issue #71
* Nullable value types now show as optional in Open API- issue #58
