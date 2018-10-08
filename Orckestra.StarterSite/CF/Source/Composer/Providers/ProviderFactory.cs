using System;
using Orckestra.Overture;

namespace Orckestra.Composer.Providers
{
    public abstract class ProviderFactory<TProvider> 
    {
        protected IDependencyResolver DependencyResolver { get; private set; }

        protected ProviderFactory(IDependencyResolver dependencyResolver)
        {
            DependencyResolver = dependencyResolver;
        }

        /// <summary>
        /// Gets the instance of a given provider type.
        /// </summary>
        /// <param name="providerType">Type to resolve.</param>
        /// <returns></returns>
        protected TProvider GetProviderInstance(Type providerType)
        {
            var instance = (TProvider) DependencyResolver.Resolve(providerType);
            return instance;
        }
    }


}
