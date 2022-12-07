// Required for CQRS module of GedasFX.SourceGenerators.gRPC.MediatR

namespace MediatR
{
    public interface ICommandRequest<out TResponse> : IRequest<TResponse>, IBaseCommandRequest { }
    public interface IQueryRequest<out TResponse> : IRequest<TResponse>, IBaseQueryRequest { }

    public interface IBaseCommandRequest : IBaseRequest { }
    public interface IBaseQueryRequest : IBaseRequest { }
}