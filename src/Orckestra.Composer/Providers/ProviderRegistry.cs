using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Providers
{
    public class ProviderRegistry<T>
    {
        private readonly ConcurrentDictionary<string, Type> _registeredProviders;

        protected ProviderRegistry()
        {
            _registeredProviders = new ConcurrentDictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Registers a provider with the given name to resolve the given type.
        /// </summary>
        /// <param name="name">Name that will be used to resolve the provider.</param>
        /// <typeparam name="TProvider">Type of the provider. This type needs to derive from <see cref="T"/>.</typeparam>
        public void RegisterProvider<TProvider>(string name) where TProvider : T
        {
            RegisterProvider(name, typeof(TProvider));
        }

        /// <summary>
        /// Registers a provider with the given name to resolve the given type.
        /// </summary>
        /// <param name="name">Name that will be used to resolve the provider.</param>
        /// <param name="providerType">Type of the provider. This type needs to derive from <see cref="T"/>.</param>
        public void RegisterProvider(string name, Type providerType)
        {
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(), nameof(name)); }
            if (providerType == null) { throw new ArgumentNullException(nameof(providerType)); }

            if (!typeof(T).IsAssignableFrom(providerType)) 
            { 
                throw new ArgumentException(string.Format("providerType must extend {0}.", 
                typeof(T).AssemblyQualifiedName), nameof(providerType)); 
            }

            _registeredProviders.AddOrUpdate(name, providerType, (s, type) => providerType);
        }

        /// <summary>
        /// Gets a registered provider type by its name. If the provider was not registered, throws an exception.
        /// </summary>
        /// <param name="name">Name of the provider.</param>
        /// <returns></returns>
        public Type ResolveProviderType(string name)
        {
            if (_registeredProviders.Count == 0)
            {
                throw new InvalidOperationException("No providers have been registered. Please add at least one.");
            }

            if (!_registeredProviders.TryGetValue(name, out Type type))
            {
                throw new ArgumentException(string.Format("'{0}' didn't match any registered provider.", name), "name");
            }

            return type;
        }

        /// <summary>
        /// Determines if a provider was registered.
        /// </summary>
        /// <param name="name">Name of the provider.</param>
        /// <returns></returns>
        public bool IsProviderRegistered(string name)
        {
            return _registeredProviders.ContainsKey(name);
        }

        /// <summary>
        /// Obtains the types of all registered providers.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<string, Type> GetAllRegisteredProviderTypes()
        {
            return new ReadOnlyDictionary<string, Type>(_registeredProviders);
        }
    }
}
