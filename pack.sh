dotnet tool install -g dotnet-script
dotnet script setNuspecVersion.csx
dotnet pack ./Source/FunctionMonkey/FunctionMonkey.csproj --output ./ --configuration Release
dotnet pack ./Source/FunctionMonkey.Abstractions/FunctionMonkey.Abstractions.csproj --output ./ --configuration Release
dotnet pack ./Source/FunctionMonkey.Commanding.Abstractions/FunctionMonkey.Commanding.Abstractions.csproj --output ./ --configuration Release
dotnet pack ./Source/FunctionMonkey.Commanding.Cosmos.Abstractions/FunctionMonkey.Commanding.Cosmos.Abstractions.csproj --output ./ --configuration Release
dotnet pack ./Source/FunctionMonkey.Compiler/FunctionMonkey.Compiler.csproj --output ./ --configuration Release
dotnet pack ./Source/FunctionMonkey.FluentValidation/FunctionMonkey.FluentValidation.csproj --output ./ --configuration Release
dotnet pack ./Source/FunctionMonkey.SignalR/FunctionMonkey.SignalR.csproj --output ./ --configuration Release
dotnet pack ./Source/FunctionMonkey.Testing/FunctionMonkey.Testing.csproj --output ./ --configuration Release
dotnet pack ./Source/FunctionMonkey.TokenValidator/FunctionMonkey.TokenValidator.csproj --output ./ --configuration Release
dotnet pack ./Source/FunctionMonkey.AspNetCore/FunctionMonkey.AspNetCore.csproj --output ./ --configuration Release
dotnet pack ./Source/FunctionMonkey.FSharp/FunctionMonkey.FSharp.fsproj --output ./ --configuration Release
dotnet pack ./Source/FunctionMonkey.MediatR/FunctionMonkey.MediatR.csproj --output ./ --configuration Release
