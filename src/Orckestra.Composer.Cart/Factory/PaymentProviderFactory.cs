using System;
using System.Collections.Generic;
using Orckestra.Composer.Cart.Providers.Payment;
using Orckestra.Composer.Providers;
using Orckestra.Overture;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Factory
{
    public sealed class PaymentProviderFactory : ProviderFactory<IPaymentProvider>, IPaymentProviderFactory
    {
        private IPaymentProviderRegistry PaymentProviderRegistry { get; set; }

        public PaymentProviderFactory(IDependencyResolver dependencyResolver, IPaymentProviderRegistry paymentProviderRegistry) 
            : base(dependencyResolver)
        {
            PaymentProviderRegistry = paymentProviderRegistry;
        }

        /// <summary>
        /// Resolves a payment provider by its name.
        /// </summary>
        /// <param name="name">Name of the Payment provider to resolve.</param>
        /// <returns></returns>
        public IPaymentProvider ResolveProvider(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(), nameof(name)); }

            Type providerType = PaymentProviderRegistry.ResolveProviderType(name);

            var instance = GetProviderInstance(providerType);
            instance.ProviderName = name;

            return instance;
        }

        /// <summary>
        /// Resolves all payment providers registered.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IPaymentProvider> ResolveAllProviders()
        {
            var providers = PaymentProviderRegistry.GetAllRegisteredProviderTypes();

            foreach (var p in providers)
            {
                var instance = GetProviderInstance(p.Value);
                instance.ProviderName = p.Key;

                yield return instance;
            }
        }
    }
}
