using GedasFX.SourceGenerators.gRPC.MediatR.Tests.Features;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace GedasFX.SourceGenerators.gRPC.MediatR.Tests;

public class MediatorTests
{
    [Fact]
    public async Task TestRpcCompiles()
    {
        var container = new ServiceCollection().AddMediatR(typeof(HelloRequestHandler)).BuildServiceProvider();
        var service = new MyOrg.Application.Greeter.GreeterService(container.GetRequiredService<IMediator>());

        var response = await service.SayHello(new MyOrg.Application.HelloRequest { Name = "Tester" }, new Mock<ServerCallContext>().Object);

        Assert.Equal("Hello Tester", response.Message);
    }
}