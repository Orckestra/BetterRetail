using System;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Configuration;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.Providers.MonerisPayment.ServiceModel;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Repositories
{
    public class VaultProfileRepository : IVaultProfileRepository
    {
        /// <summary>
        /// Gets the overture client.
        /// </summary>
        /// <value>
        /// The overture client.
        /// </value>
        protected IOvertureClient OvertureClient { get; private set; }

        /// <summary>
        /// Gets the cache provider.
        /// </summary>
        /// <value>
        /// The cache provider.
        /// </value>
        protected ICacheProvider CacheProvider { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VaultProfileRepository"/> class.
        /// </summary>
        /// <param name="overtureClient">The overture client.</param>
        /// <param name="cacheProvider">The cache provider.</param>
        /// <exception cref="System.ArgumentNullException">
        /// overtureClient
        /// or
        /// cacheProvider
        /// </exception>
        public VaultProfileRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        /// <summary>
        /// Adds the credit card.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">param</exception>
        /// <exception cref="System.ArgumentException">
        /// CardHolderName may not be null or whitespace.;param
        /// or
        /// CartName may not be null or whitespace.;param
        /// or
        /// CustomerId may not be an empty guid.;param
        /// or
        /// PaymentId may not be an empty guid.;param
        /// or
        /// TemporaryToken may not be null or whitespace.;param
        /// or
        /// IPAddress may not be null or whitespace.;param
        /// or
        /// ScopeId may not be null or whitespace.;param
        /// </exception>
        public virtual async Task<VaultProfileCreationResult> AddCreditCardAsync(AddCreditCardParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            if (string.IsNullOrWhiteSpace(param.CardHolderName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CardHolderName)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (param.PaymentId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.PaymentId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.VaultTokenId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.VaultTokenId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.IpAddress)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.IpAddress)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.ScopeId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.ScopeId)), nameof(param)); }

            var request = new CreateCartPaymentVaultProfileRequest
            {
                CardHolderName = param.CardHolderName,
                CartName = param.CartName,
                CustomerId = param.CustomerId,
                PaymentId = param.PaymentId,
                TemporaryToken = param.VaultTokenId,
                IP = param.IpAddress,
                ScopeId = param.ScopeId,
                CreatePaymentProfile = param.CreatePaymentProfile
            };

            await CacheProvider.RemoveAsync(BuildPaymentMethodCacheKey(param.ScopeId, param.CartName, param.CustomerId, param.PaymentProviderName)).ConfigureAwait(false);

            return await OvertureClient.SendAsync(request);
        }

        protected virtual CacheKey BuildPaymentMethodCacheKey(string scope, string cartName, Guid customerId, string providerName)
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
    }
}
