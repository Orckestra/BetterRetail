using Orckestra.Composer.Exceptions;
using Orckestra.Composer.Logging;
using Orckestra.ExperienceManagement.Configuration;
using ServiceStack;
using ServiceStack.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Orckestra.Composer
{
    public sealed class ComposerOvertureClient : IComposerOvertureClient
    {

        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private readonly JsonServiceClient _client;

        private ComposerOvertureClient(JsonServiceClient client)
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
        
        public Task<HttpWebResponse> SendAsync(IReturnVoid requestDto)
        {
            return InterceptAsync(() => _client.SendAsync<HttpWebResponse>(requestDto), requestDto);
        }
        
        public void Send(IReturnVoid requestDto, string httpMethod)
        {
            Intercept(() => _client.Send(requestDto), requestDto);
        }

        public Task<TResponse> SendAsync<TResponse>(IReturn<TResponse> requestDto)
        {
            return InterceptAsync(() => _client.SendAsync(requestDto), requestDto);
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
                if (e is WebServiceException wsException)
                {
                    if (wsException.StatusCode == (int)HttpStatusCode.NotFound)
                        return await Task.FromResult<T>(default);
                }

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
            if (!(ComposerHost.Current?.IsInitialized ?? false)) { return null; }

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


        public static IComposerOvertureClient CreateFromConfig()
        {
            var settings = OvertureConfiguration.Settings;
            var client = new JsonServiceClient(settings.Url);
            client.Headers.Add("X-Auth", settings.AuthToken);
            return new ComposerOvertureClient(client);
        }
    }
}