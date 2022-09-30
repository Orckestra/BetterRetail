using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Orckestra.Composer.Caching
{
    public interface ICacheClientRegistry
    {
        void ClearAll();
        void RegisterClient(string entryName, ICacheClient cacheClient, CacheItemPriority priority, TimeSpan duration, TimeSpan acquiredLockTimeout);
        void RegisterClient(string categoryName, CacheProfile profile);
        void RegisterClient(string categoryName, string profileName);
        void ApplyConfiguration(CacheConfiguration configuration);
        bool CategoryIsDefined(string categoryName);
        void RegisterDefaultClient(ICacheClient cacheClient, CacheItemPriority priority, TimeSpan duration, TimeSpan acquiredLockTimeout);

        CacheItemPolicyInfo GetCacheItemPolicyInfo(CacheKey cacheKey, bool returnDefault);

        IEnumerable<KeyValuePair<string, CacheItemPolicyInfo>> CacheItemPolicyInfos { get; }

    }

    public class CacheClientRegistry : ICacheClientRegistry
    {
        private static readonly CacheClientRegistry _instance = new CacheClientRegistry();

        public static CacheClientRegistry Instance
        {
            get { return _instance; }
        }

        public CacheClientRegistry()
        {
            _cacheClients = new ConcurrentDictionary<string, CacheItemPolicyInfo>();
            _defaultCacheClient = new CacheItemPolicyInfo(NullCacheClient.Default);
        }

        private readonly ConcurrentDictionary<string, CacheItemPolicyInfo> _cacheClients;
        private CacheItemPolicyInfo _defaultCacheClient;

        public CacheItemPolicyInfo GetCacheItemPolicyInfo(CacheKey cacheKey, bool returnDefault)
        {
            if (returnDefault)
            {
                return _cacheClients.GetOrAdd(cacheKey.CategoryName, s => _defaultCacheClient);
            }
            CacheItemPolicyInfo policy;
            _cacheClients.TryGetValue(cacheKey.CategoryName, out policy);
            return policy;
        }

        public void ClearAll()
        {
            //_defaultCacheClient.CacheClient.Clear();
            foreach (var cacheClient in _cacheClients.Values)
            {
                try
                {
                    //cacheClient..Clear();
                    //todo JFL
                }
                catch (NotSupportedException)
                {
                    //the cache client does not support the Clear operation
                    //ignore and continue
                }
            }
        }

        public IEnumerable<KeyValuePair<string, CacheItemPolicyInfo>> CacheItemPolicyInfos
        {
            get { return _cacheClients; }
        }

        public void RegisterClient(string entryName, ICacheClient cacheClient, CacheItemPriority priority, TimeSpan duration, TimeSpan acquiredLockTimeout)
        {
            _cacheClients[entryName] = new CacheItemPolicyInfo(cacheClient)
            {
                CacheItemPriority = priority,
                Duration = duration,
                AcquiredLockTimeout = acquiredLockTimeout
            };
        }

        public void RegisterClient(string categoryName, CacheProfile profile)
        {
            var cacheProfileInfo = CreateProfileCacheInfo(profile);
            _cacheClients[categoryName] = cacheProfileInfo;
        }

        public void RegisterClient(string categoryName, string profileName)
        {
            RegisterClient(categoryName, CacheConfiguration.Instance.Profiles[profileName]);
        }

        public void ApplyConfiguration(CacheConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");

            if (!string.IsNullOrWhiteSpace(configuration.Profiles.Default))
            {
                var profile = configuration.Profiles.Cast<CacheProfile>()
                        .FirstOrDefault(p => string.Equals(p.Name, configuration.Profiles.Default));

                if (profile == null)
                {
                    throw new InvalidOperationException(string.Format("Profile {0} was not found.", configuration.Profiles));
                }

                _defaultCacheClient = CreateProfileCacheInfo(profile);
            }

            foreach (CacheCategory category in configuration.Categories)
            {
                var profile = configuration.Profiles.Cast<CacheProfile>()
                        .FirstOrDefault(p => string.Equals(p.Name, category.ProfileName));

                if (profile == null)
                {
                    throw new InvalidOperationException(string.Format("Profile {0} was not found.", category.ProfileName));
                }

                RegisterClient(category.Name, profile);
            }
        }

        public bool CategoryIsDefined(string categoryName)
        {
            return _cacheClients.ContainsKey(categoryName);
        }

        public void RegisterDefaultClient(ICacheClient cacheClient, CacheItemPriority priority, TimeSpan duration, TimeSpan acquiredLockTimeout)
        {
            _defaultCacheClient = new CacheItemPolicyInfo(cacheClient)
            {
                CacheItemPriority = priority,
                Duration = duration,
                AcquiredLockTimeout = acquiredLockTimeout
            };
        }


        private static CacheItemPolicyInfo CreateProfileCacheInfo(CacheProfile profile)
        {
            var cacheClientType = Type.GetType(profile.ClientType);
            if (cacheClientType == null)
            {
                throw new InvalidOperationException(string.Format("CacheClient type {0} was not found.",
                    profile.ClientType));
            }

            var cacheProfileInfo = new CacheItemPolicyInfo(cacheClientType);

            cacheProfileInfo.Duration = profile.Duration;
            cacheProfileInfo.AcquiredLockTimeout = profile.AcquiredLockTimeout;

            if (profile.Settings != null && profile.Settings.Count > 0)
            {
                var settings = profile.Settings
                    .Cast<ProfileSetting>()
                    .ToDictionary(s => s.Key, s => s.Value);
                cacheProfileInfo.CacheClientSettings = settings;
            }

            cacheProfileInfo.CacheItemPriority = profile.Priority == ConfigurationCachePriority.NotRemovable
                ? CacheItemPriority.NotRemovable
                : CacheItemPriority.Normal;
            return cacheProfileInfo;
        }
    }
}
