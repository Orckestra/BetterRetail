using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Utils;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.Customers;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping;
using Orckestra.Overture.ServiceModel.Requests.Orders.Shopping.Payments;

namespace Orckestra.Composer.Cart.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        protected IOvertureClient OvertureClient { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }

        public PaymentRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            if (overtureClient == null) { throw new ArgumentNullException(nameof(overtureClient)); }
            if (cacheProvider == null) { throw new ArgumentNullException(nameof(cacheProvider)); }

            OvertureClient = overtureClient;
            CacheProvider = cacheProvider;
        }

        /// <summary>
        /// Gets all payments hold by a cart.
        /// </summary>
        /// <param name="param">Parameters used to make the query.</param>
        /// <returns>A list of Payments.</returns>
        public virtual Task<List<Payment>> GetCartPaymentsAsync(GetCartPaymentsParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CartName"), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CustomerId"), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), nameof(param)); }
            var request = new GetPaymentsInCartRequest
            {
                CartName = param.CartName,
                CultureName = param.CultureInfo.Name,
                CustomerId = param.CustomerId,
                ScopeId = param.Scope
            };
            return OvertureClient.SendAsync(request);
        }

        /// <summary>
        /// Get the Payment methods available for a cart.
        /// </summary>
        /// <param name="param">GetPaymentMethodsParam</param>
        /// <returns>A List of PaymentMethod</returns>
        public virtual async Task<List<PaymentMethod>> GetPaymentMethodsAsync(GetPaymentMethodsParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CartName"), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), nameof(param)); }
            if (param.ProviderNames == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("ProviderNames"), nameof(param)); }

            if (!param.ProviderNames.Any())
            {
                return new List<PaymentMethod>();
            }

            var tasks = param.ProviderNames.Select(pName => GetPaymentMethodForProviderAsync(param, pName)).ToArray();
            await Task.WhenAll(tasks).ConfigureAwait(false);

            var methods = tasks.SelectMany(t => t.Result).ToList();
            return methods;
        }

        /// <summary>
        /// Obtains the available payment methods for a given provider.
        /// </summary>
        /// <param name="param">Parameters used to make the request.</param>
        /// <param name="providerName">Provider to use in the request.</param>
        /// <returns></returns>
        protected virtual Task<List<PaymentMethod>> GetPaymentMethodForProviderAsync(GetPaymentMethodsParam param,
            string providerName)
        {
            var cacheKey = BuildPaymentMethodCacheKey(param.Scope, param.CartName, param.CustomerId, providerName);


            var request = new FindCartPaymentMethodsRequest
            {
                CartName = param.CartName,
                ScopeId = param.Scope,
                PaymentProviderName = providerName,
                CustomerId = param.CustomerId
            };

            return CacheProvider.GetOrAddAsync(cacheKey, () => OvertureClient.SendAsync(request));
        }

        public virtual Task<ProcessedCart> UpdatePaymentMethodAsync(UpdatePaymentMethodParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CartName"), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.PaymentProviderName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("PaymentProviderName"), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CustomerId"), nameof(param)); }
            if (param.PaymentId == Guid.Empty) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("PaymentId"), nameof(param)); }
            if (param.PaymentMethodId == Guid.Empty) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("PaymentMethodId"), nameof(param)); }

            var cacheKey = BuildCartCacheKey(param.Scope, param.CartName, param.CustomerId);

            var request = new UpdatePaymentMethodRequest
            {
                CartName = param.CartName,
                CultureName = param.CultureInfo.Name,
                CustomerId = param.CustomerId,
                PaymentId = param.PaymentId,
                PaymentMethodId = param.PaymentMethodId,
                PaymentProviderName = param.PaymentProviderName,
                ScopeId = param.Scope
            };

            return CacheProvider.ExecuteAndSetAsync(cacheKey, () => OvertureClient.SendAsync(request));
        }

        /// <summary>
        /// Initializes the specified payment in the cart.
        /// </summary>
        /// <param name="param">Parameters used to initialized the Payment.</param>
        /// <returns>The updated processed cart.</returns>
        public virtual Task<Overture.ServiceModel.Orders.Cart> InitializePaymentAsync(InitializePaymentParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CartName"), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CustomerId"), nameof(param)); }
            if (param.PaymentId == Guid.Empty) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("PaymentId"), nameof(param)); }

            var cacheKey = BuildCartCacheKey(param.Scope, param.CartName, param.CustomerId);

            var request = new InitializePaymentRequest
            {
                AdditionalData = param.AdditionalData == null ? null : new PropertyBag(param.AdditionalData),
                CartName = param.CartName,
                CultureName = param.CultureInfo.Name,
                CustomerId = param.CustomerId,
                Options = param.Options == null ? null : new PropertyBag(param.Options),
                PaymentId = param.PaymentId,
                ScopeId = param.Scope
            };

            return CacheProvider.ExecuteAndSetAsync(cacheKey, () => OvertureClient.SendAsync(request));
        }

        /// <summary>
        /// Voids a payment and generates a new one.
        /// </summary>
        /// <param name="param">Parameters used to void the payment.</param>
        /// <returns>The updated cart.</returns>
        public virtual Task<Overture.ServiceModel.Orders.Cart> VoidPaymentAsync(VoidOrRemovePaymentParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CartName"), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), nameof(param)); }
            if (param.CustomerId == default(Guid)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CustomerId"), nameof(param)); }
            if (param.PaymentId == default(Guid)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("PaymentId"), nameof(param)); }

            var cacheKey = BuildCartCacheKey(param.Scope, param.CartName, param.CustomerId);

            var request = new VoidPaymentRequest
            {
                CartName = param.CartName,
                CultureName = param.CultureInfo.Name,
                CustomerId = param.CustomerId,
                PaymentId = param.PaymentId,
                ScopeId = param.Scope
            };

            return CacheProvider.ExecuteAndSetAsync(cacheKey, () => OvertureClient.SendAsync(request));
        }

        /// <summary>
        /// Removes a payment from a cart.
        /// </summary>
        /// <param name="param">Parameters used to remove the payment.</param>
        /// <returns>The updated cart.</returns>
        public virtual Task<ProcessedCart> RemovePaymentAsync(VoidOrRemovePaymentParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CartName"), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CultureInfo"), nameof(param)); }
            if (param.CustomerId == default(Guid)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CustomerId"), nameof(param)); }
            if (param.PaymentId == default(Guid)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("PaymentId"), nameof(param)); }

            var cacheKey = BuildCartCacheKey(param.Scope, param.CartName, param.CustomerId);

            var request = new RemovePaymentRequest
            {
                CartName = param.CartName,
                CultureName = param.CultureInfo.Name,
                CustomerId = param.CustomerId,
                Id = param.PaymentId,
                ScopeId = param.Scope
            };

            return CacheProvider.ExecuteAndSetAsync(cacheKey, () => OvertureClient.SendAsync(request));
        }

        /// <summary>
        /// Remove a payment method from a user profile
        /// </summary>
        /// <param name="param">Parameters used to remove the payment method.</param>
        public virtual async Task RemovePaymentMethodAsync(RemovePaymentMethodParam param)
        {
            if (param == null) throw new ArgumentNullException(nameof(param), "param is required");
            if (param.PaymentMethodId == Guid.Empty) throw new ArgumentException("param.PaymentMethodId is required", nameof(param.PaymentMethodId));
            if (param.CustomerId == Guid.Empty) throw new ArgumentException("param.CustomerId is required", nameof(param.CustomerId));
            if (String.IsNullOrWhiteSpace(param.ScopeId)) throw new ArgumentException("param.ScopeId is required", nameof(param.ScopeId));
            if (String.IsNullOrWhiteSpace(param.CartName)) throw new ArgumentException("param.CartName is required", nameof(param.CartName));
            if (String.IsNullOrWhiteSpace(param.PaymentProviderName)) throw new ArgumentException("param.PaymentProviderName is required", nameof(param.PaymentProviderName));

            await CacheProvider.RemoveAsync(BuildPaymentMethodCacheKey(param.ScopeId, param.CartName, param.CustomerId, param.PaymentProviderName)).ConfigureAwait(false);

            await OvertureClient.SendAsync(new DeleteCustomerPaymentMethodRequest
            {
                ScopeId = param.ScopeId,
                CustomerId = param.CustomerId,
                PaymentMethodId = param.PaymentMethodId,
                PaymentProviderName = param.PaymentProviderName
            }).ConfigureAwait(false);
        }

        public virtual Task<List<PaymentProfile>> GetCustomerPaymentProfiles(GetCustomerPaymentProfilesParam param)
        {
            if (param == null) { throw new ArgumentNullException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param))); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.CustomerId))); }
            if (string.IsNullOrWhiteSpace(param.ScopeId)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.ScopeId))); }

            return OvertureClient.SendAsync(new GetCustomerPaymentProfilesRequest
            {
                CustomerId = param.CustomerId,
                ScopeId = param.ScopeId
            });
        }

        public Task<List<PaymentMethod>> GetCustomerPaymentMethodForProviderAsync(GetCustomerPaymentMethodsForProviderParam param)
        {
            if (param == null) { throw new ArgumentNullException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param))); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.CustomerId))); }
            if (string.IsNullOrWhiteSpace(param.ScopeId)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.ScopeId))); }
            if (string.IsNullOrWhiteSpace(param.ProviderName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.ProviderName))); }

            var getCustomerPaymentMethodsRequest = new GetCustomerPaymentMethodsRequest
            {
                CustomerId = param.CustomerId,
                PaymentProviderName = param.ProviderName,
                ScopeId = param.ScopeId
            };

            return OvertureClient.SendAsync(getCustomerPaymentMethodsRequest);
        }
        public Task<Payment> GetPaymentAsync(GetPaymentParam param)
        {
            if (param == null) { throw new ArgumentNullException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param))); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.CustomerId))); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.Scope))); }
            if (param.CultureInfo == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.CultureInfo))); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage(nameof(param.CartName))); }

            var getPaymentRequest = new GetPaymentRequest
            {
                CustomerId = param.CustomerId,
                CartName = param.CartName,
                ScopeId = param.Scope,
                CultureName = param.CultureInfo.Name,
                Id = param.PaymentId
            };

            return OvertureClient.SendAsync(getPaymentRequest);
        }

        /// <summary>
        /// Builds a cache key for a cart operation.
        /// </summary>
        /// <param name="scope">Scope of the request.</param>
        /// <param name="cartName">Name of the cart.</param>
        /// <param name="customerId">ID of the customer to which belongs the cart.</param>
        /// <returns></returns>
        protected virtual CacheKey BuildCartCacheKey(string scope, string cartName, Guid customerId)
        {
            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.Cart)
            {
                Scope = scope
            };

            cacheKey.AppendKeyParts(cartName);
            cacheKey.AppendKeyParts(customerId);

            return cacheKey;
        }

        protected CacheKey BuildPaymentMethodCacheKey(string scope, string cartName, Guid customerId, string providerName)
        {            
            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.PaymentMethod)
            {
                Scope = scope,
            };
            cacheKey.AppendKeyParts(cartName);
            cacheKey.AppendKeyParts(providerName);
            cacheKey.AppendKeyParts(customerId);

            return cacheKey;
        }

        /// <summary>
        /// Builds the cache key for a cart payment.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="customerId"></param>
        /// <param name="cartName"></param>
        /// <returns></returns>
        protected virtual CacheKey BuildCartPaymentCacheKey(string scope, Guid customerId, string cartName)
        {
            var key = new CacheKey(CacheConfigurationCategoryNames.CartPayment)
            {
                Scope = scope
            };

            key.AppendKeyParts(customerId, cartName);
            return key;
        }

    }
}
