using System;

namespace Orckestra.Composer.Search.Providers.FacetPredicate
{
    public interface IFacetPredicateProviderRegistry
    {
        /// <summary>
        /// Registers a provider with the given name to resolve the given type.
        /// </summary>
        /// <param name="name">Name that will be used to resolve the provider.</param>
        /// <typeparam name="TProvider">Type of the provider. This type needs to derive from <see cref="IFacetPredicateProvider"/>.</typeparam>
        void RegisterProvider<TProvider>(string name) where TProvider : IFacetPredicateProvider;

        /// <summary>
        /// Registers a provider with the given name to resolve the given type.
        /// </summary>
        /// <param name="name">Name that will be used to resolve the provider.</param>
        /// <param name="providerType">Type of the provider. This type needs to derive from <see cref="IFacetPredicateProvider"/>.</param>
        void RegisterProvider(string name, Type providerType);

        /// <summary>
        ///     Gets a registered provider type by its name. If the provider was not registered, throws an exception.
        /// </summary>
        /// <param name="name">Name of the provider.</param>
        /// <returns></returns>
        Type ResolveProviderType(string name);
    }
}