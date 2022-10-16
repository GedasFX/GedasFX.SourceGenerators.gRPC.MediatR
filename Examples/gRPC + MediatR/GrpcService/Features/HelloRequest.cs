using MediatR;

namespace GrpcService.Features;

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