using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Threading.Tasks;
using Orckestra.Composer.Exceptions;
using Orckestra.Composer.Logging;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.Overture;
using Orckestra.Overture.RestClient;
using ServiceStack;
using ServiceStack.Validation;

namespace Orckestra.Composer
{
    public sealed class ComposerOvertureClient : IOvertureClient
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private readonly IOvertureClient _client;

        private ComposerOvertureClient(IOvertureClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public TResponse Send<TResponse>(IReturn<TResponse> request)
        {
            return Intercept(() => _client.Send(request), request);
        }

        public void Send(IReturnVoid requestDto)
        {
            Intercept(() => _client.Send(requestDto), requestDto);
        }

        public Task<TResponse> SendAsync<TResponse>(IReturn<TResponse> requestDto)
        {
            return InterceptAsync(() => _client.SendAsync(requestDto), requestDto);
        }

        public Task<HttpWebResponse> SendAsync(IReturnVoid requestDto)
        {
            return InterceptAsync(() => _client.SendAsync(requestDto), requestDto);
        }

        public TResponse Send<TResponse>(IReturn<TResponse> request, string httpMethod)
        {
            return Intercept(() => _client.Send(request, httpMethod), request);
        }

        public void Send(IReturnVoid requestDto, string httpMethod)
        {
            Intercept(() => _client.Send(requestDto, httpMethod), requestDto);
        }

        public Task<TResponse> SendAsync<TResponse>(IReturn<TResponse> requestDto, string httpMethod)
        {
            return InterceptAsync(() => _client.SendAsync(requestDto, httpMethod), requestDto);
        }

        public Task<HttpWebResponse> SendAsync(IReturnVoid requestDto, string httpMethod)
        {
            return InterceptAsync(() => _client.SendAsync(requestDto, httpMethod), requestDto);
        }

        public void SendAllOneWay(IEnumerable<IReturnVoid> request)
        {
            Intercept(() => _client.SendAllOneWay(request), request);
        }

        public List<TResponse> SendAll<TResponse>(IEnumerable<IReturn<TResponse>> request)
        {
            return Intercept(() => _client.SendAll(request), request);
        }

        public Task<List<TResponse>> SendAllAsync<TResponse>(IEnumerable<IReturn<TResponse>> request)
        {
            return InterceptAsync(() => _client.SendAllAsync(request), request);
        }

        public Task<HttpWebResponse> SendAllOneWayAsync<TResponse>(IEnumerable<TResponse> requestDto) where TResponse : IReturnVoid
        {
            return InterceptAsync(() => _client.SendAllOneWayAsync(requestDto), requestDto);
        }

        private void Intercept(Action action, object request)
        {
            try
            {
                using (MeasureExecutionTime(request))
                {
                    action();
                }
            }
            catch (Exception e)
            {
                HandleException(e, request);
                throw;
            }
        }

        private T Intercept<T>(Func<T> function, object request)
        {
            try
            {
                using (MeasureExecutionTime(request))
                {
                    return function();
                }
            }
            catch (Exception e)
            {
                HandleException(e, request);
                throw;
            }
        }


        private async Task<T> InterceptAsync<T>(Func<Task<T>> asyncFunction, object request)
        {
            try
            {
                using (MeasureExecutionTime(request))
                {
                    return await asyncFunction().ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                HandleException(e, request);
                throw;
            }
        }


        private string GetRequestErrorDescription(object request)
        {
            string type = request != null ? request.GetType().FullName : "null";
            return $"Failed to execute request of type '{type}'.";
        }

        private void HandleException(Exception e, object request)
        {
            if (e is ValidationError vError)
            {
                Log.ErrorException($"{GetRequestErrorDescription(request)} ErrorCode: {vError.ErrorCode}, ErrorMessage: {vError.ErrorMessage}", vError);
                ComposerException.Create(vError).ThrowIfAnyError();
            }
            else if (e is WebServiceException wsException)
            {
                Log.ErrorException($"{GetRequestErrorDescription(request)} ErrorCode: {wsException.ErrorCode}, ErrorMessage: {wsException.ErrorMessage}", wsException);
                ComposerException.Create(wsException).ThrowIfAnyError();
            }
            else if (e is WebException webException)
            {
                Log.ErrorException($"{GetRequestErrorDescription(request)} {e.Message}", e);
                ComposerException.Create(webException).ThrowIfAnyError();
            }
        }


        private IDisposable MeasureExecutionTime(object request)
        {
            var collector = ComposerHost.Current.TryResolve<IPerformanceDataCollector>();
            if (collector == null) { return null; }

            string description = "OCS API Call: ";
            if (request is IEnumerable)
            {
                description += ((IEnumerable)request).Cast<object>().Select(r => r.GetType().FullName).Join(", ");
            }
            else
            {
                description += request.GetType().FullName;
            }

            return collector.Measure(description);
        }


        public static IOvertureClient CreateFromConfig()
        {
            var config = GetClientConfig();
            return new ComposerOvertureClient(new OvertureClient(config));
        }


        private static OvertureClientConfig GetClientConfig()
        {
            var config = OvertureConfiguration.Settings;
            var clientConfig = new OvertureClientConfig
            {
                AuthToken = config.AuthToken,
                Format = ClientFormat.Json,
                ServerBaseUrl = config.Url,
                CacheLevel = HttpRequestCacheLevel.Default
            };

            return clientConfig;
        }
    }
}