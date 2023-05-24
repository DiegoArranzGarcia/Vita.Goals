using FastEndpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vita.Goals.FunctionalTests.Extensions;
public static class HttpClientExtensions
{
    /// <summary>
    /// make a PATCH request using a request dto and get back a response dto.
    /// </summary>
    /// <typeparam name="TRequest">type of the requet dto</typeparam>
    /// <typeparam name="TResponse">type of the response dto</typeparam>
    /// <param name="requestUri">the route url to post to</param>
    /// <param name="request">the request dto</param>
    public static Task<TestResult<TResponse>> PATCHAsync<TRequest, TResponse>(this HttpClient client,
                                                                              string requestUri,
                                                                              TRequest request)
        => client.Send<TRequest, TResponse>(HttpMethod.Patch, requestUri, request);

    /// <summary>
    /// make a PATCH request to an endpoint using auto route discovery using a request dto and get back a response dto.
    /// </summary>
    /// <typeparam name="TEndpoint">the type of the endpoint</typeparam>
    /// <typeparam name="TRequest">the type of the request dto</typeparam>
    /// <typeparam name="TResponse">the type of the response dto</typeparam>
    /// <param name="request">the request dto</param>
    public static Task<TestResult<TResponse>> PATCHAsync<TEndpoint, TRequest, TResponse>(this HttpClient client,
                                                                                         TRequest request) where TEndpoint : IEndpoint
        => PATCHAsync<TRequest, TResponse>(client, IEndpoint.TestURLFor<TEndpoint>(), request);

    /// <summary>
    /// make a PATCH request to an endpoint using auto route discovery using a request dto that does not send back a response dto.
    /// </summary>
    /// <typeparam name="TEndpoint">the type of the endpoint</typeparam>
    /// <typeparam name="TRequest">the type of the request dto</typeparam>
    /// <param name="request">the request dto</param>
    public static async Task<HttpResponseMessage> PATCHAsync<TEndpoint, TRequest>(this HttpClient client,
                                                                                  TRequest request) where TEndpoint : IEndpoint
    {
        var (rsp, _) = await PATCHAsync<TRequest, EmptyResponse>(client, IEndpoint.TestURLFor<TEndpoint>(), request);
        return rsp;
    }

    /// <summary>
    /// make a PATCH request to an endpoint using auto route discovery without a request dto and get back a typed response dto.
    /// </summary>
    /// <typeparam name="TEndpoint">the type of the endpoint</typeparam>
    /// <typeparam name="TResponse">the type of the response dto</typeparam>
    public static Task<TestResult<TResponse>> PATCHAsync<TEndpoint, TResponse>(this HttpClient client) where TEndpoint : IEndpoint
        => PATCHAsync<EmptyRequest, TResponse>(client, IEndpoint.TestURLFor<TEndpoint>(), new EmptyRequest());
}
