using MediatR;
using MyOrg.Application;

namespace GedasFX.SourceGenerators.gRPC.MediatR.Tests.Features;

public class HelloRequestHandler : IRequestHandler<HelloRequest, HelloReply>
{
    public Task<HelloReply> Handle(HelloRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
}