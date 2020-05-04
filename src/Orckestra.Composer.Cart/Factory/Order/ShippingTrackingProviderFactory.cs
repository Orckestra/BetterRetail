using System;
using System.Collections.Generic;
using Orckestra.Composer.Cart.Providers.ShippingTracking;
using Orckestra.Composer.Providers;
using Orckestra.Overture;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Factory.Order
{
    public class ShippingTrackingProviderFactory : ProviderFactory<IShippingTrackingProvider>, IShippingTrackingProviderFactory
    {
        protected IShippingTrackingProviderRegistry ShippingTrackingProviderRegistry { get; set; }

        public ShippingTrackingProviderFactory(IDependencyResolver dependencyResolver,
            IShippingTrackingProviderRegistry shippingTrackingProviderRegistry) 
            : base(dependencyResolver)
        {
            ShippingTrackingProviderRegistry = shippingTrackingProviderRegistry;
        }
     
        public IShippingTrackingProvider ResolveProvider(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(), nameof(name)); }

            Type providerType;
            if (!ShippingTrackingProviderRegistry.IsProviderRegistered(name))
            {
                providerType = ShippingTrackingProviderRegistry.ResolveProviderType(CartConfiguration.DefaultShippingTrackingProviderName);
            }
            else
            {
                providerType = ShippingTrackingProviderRegistry.ResolveProviderType(name);
            }

            var instance = GetProviderInstance(providerType);
            instance.ProviderName = name;

            return instance;
        }

        public virtual IEnumerable<IShippingTrackingProvider> ResolveAllProviders()
        {
            var providers = ShippingTrackingProviderRegistry.GetAllRegisteredProviderTypes();

            foreach (var p in providers)
            {
                var instance = GetProviderInstance(p.Value);
                instance.ProviderName = p.Key;

                yield return instance;
            }
        }
    }
}
