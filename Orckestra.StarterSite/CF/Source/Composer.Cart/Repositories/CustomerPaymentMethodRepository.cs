using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Logging;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Utils;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Customers;

namespace Orckestra.Composer.Cart.Repositories
{
    /// <summary>
    /// Repository for interacting with Customers
    /// Customers represents entities which have the ability to buy products. 
    /// </summary>
    public class CustomerPaymentMethodRepository : ICustomerPaymentMethodRepository
    {
        private static ILog Log = LogProvider.GetCurrentClassLogger();
        protected IOvertureClient OvertureClient { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }

        public CustomerPaymentMethodRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            if (overtureClient == null) { throw new ArgumentNullException("overtureClient"); }
            if (cacheProvider == null) { throw new ArgumentNullException("cacheProvider"); }

            OvertureClient = overtureClient;
            CacheProvider = cacheProvider;
        }

        /// <summary>
        /// Get the Payment methods available for a customer.
        /// </summary>
        /// <param name="param">GetPaymentMethodsParam</param>
        /// <returns>A List of PaymentMethod</returns>
        public virtual async Task<List<PaymentMethod>> GetCustomerPaymentMethodsAsync(GetCustomerPaymentMethodsParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CustomerId == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CustomerId"), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), nameof(param)); }
            if (param.ProviderNames == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("ProviderNames"), nameof(param)); }

            if (!param.ProviderNames.Any())
            {
                return new List<PaymentMethod>();
            }
         
            var tasks = param.ProviderNames.Select(pName => GetPaymentMethodForProviderAsync(param, pName)).ToArray();

            try
            {              
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (Exception e)
            {             
                Log.Warn($"GetCustomerPaymentMethodsRequest failed. {e.ToString()}");               
            }

            tasks = tasks.Where(t => !t.IsFaulted).ToArray();
            var method = tasks.SelectMany(t => t.Result).ToList();

            return method;
        }

        protected virtual Task<List<PaymentMethod>> GetPaymentMethodForProviderAsync(GetCustomerPaymentMethodsParam param,
            string providerName)
        {
            var cacheKey = BuildCustomerPaymentMethodCacheKey(param.Scope, param.CustomerId, providerName);

            var request = new GetCustomerPaymentMethodsRequest
            {
                ScopeId = param.Scope,
                PaymentProviderName = providerName,
                CustomerId = param.CustomerId
            };

            return CacheProvider.GetOrAddAsync(cacheKey, () => OvertureClient.SendAsync(request));               
        }

        protected CacheKey BuildCustomerPaymentMethodCacheKey(string scope, Guid customerId, string providerName)
        {
            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.CustomerPaymentMethod)
            {
                Scope = scope,
            };
            cacheKey.AppendKeyParts(providerName);
            cacheKey.AppendKeyParts(customerId);

            return cacheKey;
        }
    }
}
