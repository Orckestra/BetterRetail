using System.Collections.Generic;
using Orckestra.Composer.Cart.Providers.Payment;

namespace Orckestra.Composer.Cart.Factory
{
    public interface IPaymentProviderFactory
    {
        /// <summary>
        /// Resolves a payment provider by its name.
        /// </summary>
        /// <param name="name">Name of the Payment provider to resolve.</param>
        /// <returns></returns>
        IPaymentProvider ResolveProvider(string name);

        /// <summary>
        /// Resolves all payment providers registered.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPaymentProvider> ResolveAllProviders();
    }
}
