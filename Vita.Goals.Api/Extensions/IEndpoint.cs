using MinimalApi.Endpoint;

namespace Vita.Goals.Api.Extensions;
public interface IEndpoint<TResult, TRequest1, TRequest2, TRequest3, TRequest4> : IEndpoint
{
    Task<TResult> HandleAsync(TRequest1 request1, TRequest2 request2, TRequest3 request3, TRequest4 request4);
}
