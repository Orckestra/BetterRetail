using System;
using System.Collections.Generic;

namespace Orckestra.Composer.Cart.Providers.Payment
{
    public interface IPaymentProviderRegistry
    {
        /// <summary>
        /// Registers a provider with the given name to resolve the given type.
        /// </summary>
        /// <param name="name">Name that will be used to resolve the provider.</param>
        /// <typeparam name="TProvider">Type of the provider. This type needs to derive from <see cref="IPaymentProvider"/>.</typeparam>
        void RegisterProvider<TProvider>(string name) where TProvider : IPaymentProvider;

        /// <summary>
        /// Registers a provider with the given name to resolve the given type.
        /// </summary>
        /// <param name="name">Name that will be used to resolve the provider.</param>
        /// <param name="providerType">Type of the provider. This type needs to derive from <see cref="IPaymentProvider"/>.</param>
        void RegisterProvider(string name, Type providerType);

        /// <summary>
        /// Gets a registered provider type by its name. If the provider was not registered, throws an exception.
        /// </summary>
        /// <param name="name">Name of the provider.</param>
        /// <returns></returns>
        Type ResolveProviderType(string name);

        /// <summary>
        /// Determines if a provider was registered.
        /// </summary>
        /// <param name="name">Name of the provider.</param>
        /// <returns></returns>
        bool IsProviderRegistered(string name);

        /// <summary>
        /// Obtains the types of all registered providers.
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<string, Type> GetAllRegisteredProviderTypes();
    }
}
