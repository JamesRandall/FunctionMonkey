# Debugging Function Monkey

Due to how the .NET Core compilation process and Azure Functions work (there aren't the same kind of startup hooks as there are in ASP.Net) Function Monkey can be a little untuitive to debug as it uses Roslyn to compile an assembly and the appropriate metadata after the target project (the one containing the _IFunctionAppConfiguration_ implementation) has been compiled. This works absolutely fine in normal usage scenarios where it is invoked through an MSBuild task but is a little more awkward when you are in a develop / test / debug chain as the .NET Core compiler has a habit of removing the compiled assembly.

## Full End to End Debugging

The workflow outlined below will enable you to debug the full end to end process based on using Visual Studio:

1. Create a new empty Azure Functions project inside the Function Monkey solution and configure Functions as required or use one of the existing projects inside the "Scratch" folder. Rather than use the NuGet packages and reference the following assemblies:
```
FunctionMonkey
FunctionMonkey.Abstractions
FunctionMonkey.Commanding.Abstractions
FunctionMonkey.FluentValidation (optional)
```
   Also add these NuGet packages:
```
AzureFromTheTrenches.Commanding
```

2. Build the project but then open the Configuration Manager and untick the "Build" checkbox for the project. This will prevent it from being built when it is run and prevent the removal of the assembly I mentioned earlier.

3. Next we need to instruct the compiler to compile functions for our new project. Open the properties for the FunctionMonkey.Compiler project and go to the Debug tab. Ensure Launch is set to Project and then set the Application Arguments to be the path to the path to the DLL with the same name as the project you set up earlier. For example if that is called MyFunctionApp and located at c:\wip\functionMonkey\scratch\MyFunctionApp then you will need to enter:

    c:\wip\functionMonkey\scratch\MyFunctionApp\bin\Debug\netstandard2.0\bin\MyFunctionApp.dll

4. Now run the FunctionMonkey.Compiler project. Any errors will appear in the output and you can insert breakpoints etc. in the compiler code.

5. To test the function app simply run it.

6. Finally: you need to be very careful with your workflow when you edit your sample project otherwise you can get very strange results (FunctionMonkey won't be compiling what you think it is). Make sure you a) explicitly build the sample project b) run the function monkey compiler again c) run the sample project.

## Inspecting the Compiled Source

Obviously you can use a tool like [dotPeek](https://www.jetbrains.com/decompiler/) to decompile the output assembly alternatively you can use the _OutputAuthoredSource_ method on the Function Monkey builder. For example:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    commandRegistry.Register<InvoiceQueryHandler>();
                })
                .Functions(functions => functions
                    .HttpRoute("Invoice", route => route
                        .HttpFunction<InvoiceQuery>()
                    )
                )
                .OutputAuthoredSource(@"c:\wip\outputFolder");
        }
    }

This will output the source as .cs files to the specified folder.

