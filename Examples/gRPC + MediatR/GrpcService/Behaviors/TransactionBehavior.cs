using MediatR;

namespace GrpcService.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly MyRepository _repository;

    public TransactionBehavior(MyRepository repository)
    {
        _repository = repository;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IBaseQueryRequest)
            return await next();

        await _repository.BeginTransaction();
        var result = await next();
        await _repository.CommitTransaction();

        return result;
    }
}

// For example purposes only.
public class MyRepository
{
    public Task BeginTransaction()
    {
        return Task.CompletedTask;
    }

    public Task CommitTransaction()
    {
        return Task.CompletedTask;
    }
}