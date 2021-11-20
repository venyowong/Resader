using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;

namespace Resader.Api.Extensions;
public static class HttpClientExtension
{
    private static ConcurrentDictionary<string, IAsyncPolicy<HttpResponseMessage>> _dictionary =
        new ConcurrentDictionary<string, IAsyncPolicy<HttpResponseMessage>>();

    /// <summary>
    /// 基于 Polly 发起 GET 请求
    /// </summary>
    /// <param name="client"></param>
    /// <param name="requestUri"></param>
    /// <param name="timeout">超时时长，分钟</param>
    /// <returns></returns>
    public static Task<HttpResponseMessage> GetWithPolly(this HttpClient client, string requestUri, int timeout = 1)
    {
        IAsyncPolicy<HttpResponseMessage> policy = null;
        if (_dictionary.ContainsKey(requestUri))
        {
            policy = _dictionary[requestUri];
        }
        else
        {
            policy = PollyPolicies.GetHttpPolicy(new TimeSpan(0, timeout, 0));
            _dictionary.TryAdd(requestUri, policy);
        }

        return policy.ExecuteAsync(async () => await client.GetAsync(requestUri));
    }

    /// <summary>
    /// 基于 Polly 发起 POST 请求
    /// </summary>
    /// <param name="client"></param>
    /// <param name="requestUri"></param>
    /// <param name="body"></param>
    /// <param name="timeout">超时时长，分钟</param>
    /// <returns></returns>
    public static Task<HttpResponseMessage> PostWithPolly(this HttpClient client, string requestUri, object body, int timeout = 1)
    {
        IAsyncPolicy<HttpResponseMessage> policy = null;
        if (_dictionary.ContainsKey(requestUri))
        {
            policy = _dictionary[requestUri];
        }
        else
        {
            policy = PollyPolicies.GetHttpPolicy(new TimeSpan(0, timeout, 0));
            _dictionary.TryAdd(requestUri, policy);
        }

        return policy.ExecuteAsync(async () => await client.PostAsJsonAsync(requestUri, body));
    }
}
