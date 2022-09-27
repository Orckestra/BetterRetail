using Orckestra.Composer.Dependency;
using System;
using System.Collections.Generic;

namespace Orckestra.Composer.Caching
{
    /// <summary>
    ///     The caching policy for an item that was added to a cache.
    /// </summary>
    public class CacheItemPolicyInfo
    {
        /// <summary>
        ///     The synchronization root ensuring that only a single instance of the cache client is created.
        /// </summary>
        private readonly object _syncRoot = new object();

        /// <summary>
        ///     The function responsible to return an implementation of <see cref="ICacheClient" /> using the provided
        ///     <see cref="IDependencyResolver" />.
        /// </summary>
        private Func<IDependencyResolver, ICacheClient> _facFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheItemPolicyInfo"/> class.
        /// </summary>
        /// <param name="cacheClientType">
        /// The type of the cache client.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Raised if the provided cacheClientType is null.
        /// </exception>
        public CacheItemPolicyInfo(Type cacheClientType)
        {
            if (cacheClientType == null)
            {
                throw new ArgumentNullException("cacheClientType");
            }

            CacheClientType = cacheClientType;
            CacheItemPriority = CacheItemPriority.Normal;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheItemPolicyInfo"/> class.
        /// </summary>
        /// <param name="cacheClient">
        /// An instance of a class implementing the <see cref="ICacheClient"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Raised if the provided cacheClient argument is null.
        /// </exception>
        public CacheItemPolicyInfo(ICacheClient cacheClient)
        {
            if (cacheClient == null)
            {
                throw new ArgumentNullException("cacheClient");
            }

            CacheClientType = cacheClient.GetType();
            SetSingletonInstance(cacheClient);
        }

        /// <summary>
        ///     Gets the cache client type.
        /// </summary>
        public Type CacheClientType { get; private set; }

        /// <summary>
        ///     Gets or sets the cache client settings.
        /// </summary>
        public IDictionary<string, string> CacheClientSettings { get; set; }

        /// <summary>
        ///     Gets or sets the cache item priority.
        /// </summary>
        public CacheItemPriority CacheItemPriority { get; set; }

        /// <summary>
        ///     Gets or sets the caching duration of the item.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        ///     Gets or sets the time to live of the lock.
        /// </summary>
        public TimeSpan AcquiredLockTimeout { get; set; }

        /// <summary>
        /// Returns an instance of a class implementing the <see cref="ICacheClient"/>. 
        /// It will try to resolve it using the provided the singleton implementation then from the <see cref="IDependencyResolver"/> provided.
        /// If none are found a new instance will be created and set. 
        /// </summary>
        /// <param name="resolver">
        /// The <see cref="IDependencyResolver"/> that should be used to try to resolve an existing implementation.
        /// </param>
        /// <returns>
        /// The instance implementing <see cref="ICacheClient"/>.
        /// </returns>
        public ICacheClient CreateClient(IDependencyResolver resolver)
        {
            if (this._facFunc != null)
            {
                return this._facFunc(resolver);
            }

            lock (this._syncRoot)
            {
                if (this._facFunc != null)
                {
                    return this._facFunc(resolver);
                }

                var client = (ICacheClient)resolver.TryResolve(CacheClientType);
                if (client == null)
                {
                    client = (ICacheClient)Activator.CreateInstance(CacheClientType);
                    client.Initialize(CacheClientSettings);
                    SetSingletonInstance(client);
                }
                else
                {
                    this._facFunc = CreateClientFromResolver;
                    client.Initialize(CacheClientSettings);
                }

                return client;
            }
        }

        /// <summary>
        /// Set singleton instance.
        /// </summary>
        /// <param name="cacheClient">
        ///  The instance of a class implementing the <see cref="ICacheClient"/>.
        /// </param>
        private void SetSingletonInstance(ICacheClient cacheClient)
        {
            this._facFunc = ctx => cacheClient;
        }

        /// <summary>
        /// Create a new instance of caching client from resolver and initialize its settings. 
        /// </summary>
        /// <param name="resolver">
        /// The <see cref="IDependencyResolver"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ICacheClient"/>.
        /// </returns>
        private ICacheClient CreateClientFromResolver(IDependencyResolver resolver)
        {
            var cacheClient = (ICacheClient)resolver.Resolve(CacheClientType);
            cacheClient.Initialize(CacheClientSettings);
            return cacheClient;
        }
    }
}
