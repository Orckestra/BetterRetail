using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Orckestra.Composer.Exceptions;
using Orckestra.Composer.Logging;
using Orckestra.Overture;
using ServiceStack;
using ServiceStack.Validation;
using System.Configuration;
using Orckestra.Composer.Configuration;
using Orckestra.Overture.RestClient;

namespace Orckestra.Composer
{
    public sealed class ComposerOvertureClient : IOvertureClient
    {
        private static ILog Log = LogProvider.GetCurrentClassLogger();

        private readonly IOvertureClient _client;

        private ComposerOvertureClient(IOvertureClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            _client = client;
        }

        public TResponse Send<TResponse>(IReturn<TResponse> request)
        {
            return Intercept(() => _client.Send(request));
        }

        public void Send(IReturnVoid requestDto)
        {
            Intercept(() => _client.Send(requestDto));
        }

        public Task<TResponse> SendAsync<TResponse>(IReturn<TResponse> requestDto)
        {
            return InterceptAsync(() => _client.SendAsync(requestDto));
        }

        public Task<HttpWebResponse> SendAsync(IReturnVoid requestDto)
        {
            return InterceptAsync(() => _client.SendAsync(requestDto));
        }

        public TResponse Send<TResponse>(IReturn<TResponse> request, string httpMethod)
        {
            return Intercept(() => _client.Send(request, httpMethod));
        }

        public void Send(IReturnVoid requestDto, string httpMethod)
        {
            Intercept(() => _client.Send(requestDto, httpMethod));
        }

        public Task<TResponse> SendAsync<TResponse>(IReturn<TResponse> requestDto, string httpMethod)
        {
            return InterceptAsync(() => _client.SendAsync(requestDto, httpMethod));
        }

        public Task<HttpWebResponse> SendAsync(IReturnVoid requestDto, string httpMethod)
        {
            return InterceptAsync(() => _client.SendAsync(requestDto, httpMethod));
        }

        public void SendAllOneWay(IEnumerable<IReturnVoid> request)
        {
            Intercept(() => _client.SendAllOneWay(request));
        }

        public List<TResponse> SendAll<TResponse>(IEnumerable<IReturn<TResponse>> request)
        {
            return Intercept(() => _client.SendAll(request));
        }

        public Task<List<TResponse>> SendAllAsync<TResponse>(IEnumerable<IReturn<TResponse>> request)
        {
            return InterceptAsync(() => _client.SendAllAsync(request));
        }

        public Task<HttpWebResponse> SendAllOneWayAsync<TResponse>(IEnumerable<TResponse> requestDto) where TResponse : IReturnVoid
        {
            return InterceptAsync(() => _client.SendAllOneWayAsync(requestDto));
        }

        private void Intercept(Action action)
        {
            try
            {
                action();
            }
            catch (ValidationError e)
            {
                Log.ErrorException("ErrorCode: {0}, ErrorMessage: {1}", e, e.ErrorCode, e.ErrorMessage);                
                ComposerException.Create(e).ThrowIfAnyError();
                throw;
            }
            catch (WebServiceException e)
            {
                Log.ErrorException("ErrorCode: {0}, ErrorMessage: {1}", e, e.ErrorCode, e.ErrorMessage);                
                ComposerException.Create(e).ThrowIfAnyError();
                throw;
            }
            catch (WebException e)
            {
                Log.ErrorException(e.Message, e);                
                ComposerException.Create(e).ThrowIfAnyError();
                throw;
            }
        }

        private T Intercept<T>(Func<T> function)
        {
            try
            {
                return function();
            }
            catch (ValidationError e)
            {
                Log.ErrorException("ErrorCode: {0}, ErrorMessage: {1}", e, e.ErrorCode, e.ErrorMessage);
                ComposerException.Create(e).ThrowIfAnyError();
                throw;
            }
            catch (WebServiceException e)
            {
                Log.ErrorException("ErrorCode: {0}, ErrorMessage: {1}", e, e.ErrorCode, e.ErrorMessage);
                ComposerException.Create(e).ThrowIfAnyError();
                throw;
            }
            catch (WebException e)
            {
                Log.ErrorException(e.Message, e);
                ComposerException.Create(e).ThrowIfAnyError();
                throw;
            }
        }

        private async Task<T> InterceptAsync<T>(Func<Task<T>> asyncFunction)
        {
            try
            {
                return await asyncFunction().ConfigureAwait(false);
            }
            catch (ValidationError e)
            {
                Log.ErrorException("ErrorCode: {0}, ErrorMessage: {1}", e, e.ErrorCode, e.ErrorMessage);
                ComposerException.Create(e).ThrowIfAnyError();
                throw;
            }
            catch (WebServiceException e)
            {
                Log.ErrorException("ErrorCode: {0}, ErrorMessage: {1}", e, e.ErrorCode, e.ErrorMessage);
                ComposerException.Create(e).ThrowIfAnyError();
                throw;
            }
            catch (WebException e)
            {
                Log.ErrorException(e.Message, e);
                ComposerException.Create(e).ThrowIfAnyError();
                throw;
            }
        }

        public static IOvertureClient CreateFromConfig()
        {
            var config = GetClientConfig();
            return new ComposerOvertureClient(new OvertureClient(config));
        }

        private static OvertureConfigurationElement GetOvertureConfigurationFromComposerConfiguration()
        {
            var configSection = ConfigurationManager.GetSection(ComposerConfigurationSection.ConfigurationName) as ComposerConfigurationSection;

            if (configSection != null)
            {
                return configSection.OvertureConfiguration;
            }

            throw new Exception("Unable to create an instance of the OvertureClient from the configuration.");
        }

        private static OvertureClientConfig GetClientConfig()
        {
            var config = GetOvertureConfigurationFromComposerConfiguration();
            var clientConfig = new OvertureClientConfig
            {
                AuthToken = config.AuthToken,
                Format = config.Format,
                ServerBaseUrl = config.Url,
                CacheLevel = config.CacheLevel
            };

            return clientConfig;
        }
    }
}