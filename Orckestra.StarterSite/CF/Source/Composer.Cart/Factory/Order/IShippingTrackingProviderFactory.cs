using System.Collections.Generic;
using Orckestra.Composer.Cart.Providers.ShippingTracking;

namespace Orckestra.Composer.Cart.Factory.Order
{
    public interface IShippingTrackingProviderFactory
    {
        /// <summary>
        /// Resolves a shippingTracking provider by its name.
        /// </summary>
        /// <param name="name">Name of the shippingTracking provider to resolve.</param>
        /// <returns></returns>
        IShippingTrackingProvider ResolveProvider(string name);

        /// <summary>
        /// Resolves all shippingTracking providers registered.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IShippingTrackingProvider> ResolveAllProviders();
    }
}
