# Source Generators
Repository containing various .NET Core Source Generators for simplifying processes. Below you will see the documentation for each of the services.

## gRPC + MediatR

This package eliminates the need to write Service implementations by using the default implementation which relays everything to the MediatR pipeline.

### Installation

```bash
dotnet add package GedasFX.SourceGenerators.gRPC.MediatR
```

### Usage Example

**Proto File**
```proto
syntax = "proto3";  

// csharp_namespace or package is required.
option csharp_namespace = "GrpcService";
package greet;  

// The greeting service definition.  
service Greeter {  
  // Sends a greeting  
  rpc SayHello (HelloRequest) returns (HelloReply);  
}  
  
// The request message containing the user's name.  
message HelloRequest {  
  string name = 1;  
}  
  
// The response message containing the greetings.  
message HelloReply {  
  string message = 1;  
}
```

**Command Handler**
```cs
public class HelloRequestHandler : IRequestHandler<HelloRequest, HelloReply>
{
    private readonly ILogger<HelloRequestHandler> _logger;

    public HelloRequestHandler(ILogger<HelloRequestHandler> logger)
    {
        _logger = logger;
    }

    public Task<HelloReply> Handle(HelloRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request: Name = {Name}", request.Name);

        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}
```

**Startup**
```cs
builder.Services.AddMediatR(typeof(HelloRequestHandler).Assembly);
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<Greeter.GreeterService>(); // Greeter.GreeterService was generated automatically.

app.Run();
```

**Run**

NOTE: `Grpc.AspNetCore.Server.Reflection` was added for this example.
```bash
> grpcurl -d '{ \"name\": \"World\" }' localhost:7067 greet.Greeter/SayHello
{
  "message": "Hello World"
}
```

### CQRS

Version v1.2.0 adds generation support for CQRS. If the request name ends with `Command` or `Query` the interface will be generated as `ICommandRequest<out TResponse> : IRequest<TResponse>` or `IQueryRequest<out TResponse> : IRequest<TResponse>` respectively.

This separation allows treating queries and commands differently. Here is an example of skipping transaction initialization if a request was marked as a query:

```cs
    // TranactionBehavior.cs
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IBaseQueryRequest)
            return await next();

        await _repository.BeginTransaction();
        var result = await next(); // If throws, result will not be committed.
        await _repository.CommitTransaction();

        return result;
    }
```

#### Opting in

To opt into the CQRS generation feature, add these lines to the `.csproj` file where the NuGet package is imported:

```xml
  <PropertyGroup>
    <MediatR_EnableCQRS>true</MediatR_EnableCQRS>
  </PropertyGroup>

  <ItemGroup>
    <CompilerVisibleProperty Include="MediatR_EnableCQRS" />
  </ItemGroup>
```

Note: `CompilerVisibleProperty` is required for the underlying Roslyn analyzer.

Additionally, add these lines to some `.cs` file within the project (or extract to a library) to declare the CQRS contract:

```cs
namespace MediatR
{
    public interface ICommandRequest<out TResponse> : IRequest<TResponse>, IBaseCommandRequest { }
    public interface IQueryRequest<out TResponse> : IRequest<TResponse>, IBaseQueryRequest { }

    public interface IBaseCommandRequest : IBaseRequest { }
    public interface IBaseQueryRequest : IBaseRequest { }
}
```