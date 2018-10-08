using System;
using System.Collections.Generic;
using Orckestra.Composer.Cart.Providers.ShippingTracking;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Utils;
using Orckestra.Overture;

namespace Orckestra.Composer.Cart.Factory.Order
{
    public sealed class ShippingTrackingProviderFactory : ProviderFactory<IShippingTrackingProvider>, IShippingTrackingProviderFactory
    {
        private IShippingTrackingProviderRegistry ShippingTrackingProviderRegistry { get; set; }

        public ShippingTrackingProviderFactory(IDependencyResolver dependencyResolver,
            IShippingTrackingProviderRegistry shippingTrackingProviderRegistry) 
            : base(dependencyResolver)
        {
            ShippingTrackingProviderRegistry = shippingTrackingProviderRegistry;
        }
     
        public IShippingTrackingProvider ResolveProvider(string name)
        {
            if (String.IsNullOrWhiteSpace(name)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("name"), "name"); }

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

        public IEnumerable<IShippingTrackingProvider> ResolveAllProviders()
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
