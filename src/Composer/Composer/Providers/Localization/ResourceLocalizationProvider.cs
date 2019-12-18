using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Hosting;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Kernel;
using Orckestra.Composer.Utils;
using Orckestra.Overture.Caching;
using CacheItemPriority = System.Web.Caching.CacheItemPriority;

namespace Orckestra.Composer.Providers.Localization
{
    /// <summary>
    /// This object 
    /// </summary>
    internal class ResourceLocalizationProvider : ILocalizationProvider
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly ILocalizationRepository<VirtualFile> _localizationRepository;
        private readonly VirtualPathProvider _virtualPathProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceLocalizationProvider"/> class.
        /// </summary>
        public ResourceLocalizationProvider(IComposerEnvironment environment, ICacheProvider cacheProvider)
        {
            if (environment == null) { throw new ArgumentNullException("environment"); }
            if (cacheProvider == null) { throw new ArgumentNullException("cacheProvider"); }

            _cacheProvider = cacheProvider;
            _virtualPathProvider = environment.VirtualPathProvider;
            _localizationRepository = new ResxLocalizationRepository(environment);
        }

        /// <summary>
        /// Combine all possible sources of localized string for a given (culture)
        /// into a single Tree containing the most relevent localizations.
        /// 
        /// This The localization a found in Resx files
        /// The Possible sources are resx files configured in most relevent file first
        /// <example>
        /// ComposerConfiguration.ResxLocalizationRepositoryConfiguration.PatternsForPossibleSources = {
        ///     "{category}_Custom.{cultureName}.resx",
        ///     "{category}_Custom.{twoLetterISOLanguageName}.resx",
        ///     "{category}_Custom.resx",
        ///     "{category}.{cultureName}.resx",
        ///     "{category}.{twoLetterISOLanguageName}.resx",
        ///     "{category}.resx",
        /// };
        /// </example>
        /// 
        /// <example>
        /// string value = await tree.LocalizedCategories[category].LocalizedValues[key].ConfigureAwait(false);
        /// </example>
        /// </summary>
        /// <param name="culture"></param>
        /// <returns>Tree of most relevant localized values</returns>
        public Task<LocalizationTree> GetLocalizationTreeAsync(CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }

            CacheKey localizationTreeCacheKey = new CacheKey(CacheConfigurationCategoryNames.LocalizationTree)
            {
                CultureInfo = culture,
            };

            var value = _cacheProvider.GetOrAddAsync(localizationTreeCacheKey, async () =>
            {
                List<string> fileDependencies = new List<string>();
                LocalizationTree tree = new LocalizationTree(culture);

                foreach (var categoryName in _localizationRepository.GetAllCategories())
                {
                    LocalizationCategory category = new LocalizationCategory(categoryName);

                    foreach (var source in _localizationRepository.GetPrioritizedSources(categoryName, culture))
                    {
                        Dictionary<string, string> values =
                            await _localizationRepository.GetValuesAsync(source).ConfigureAwait(false);
                        foreach (var kvp in values)
                        {
                            if (!category.LocalizedValues.ContainsKey(kvp.Key))
                            {
                                category.LocalizedValues.Add(kvp.Key, kvp.Value);
                            }
                        }

                        fileDependencies.Add(source.VirtualPath);
                    }

                    tree.LocalizedCategories.Add(categoryName.ToLowerInvariant(), category);
                }

                MonitorFileChanges(localizationTreeCacheKey, fileDependencies);

                return tree;
            });

            return value;
        }

        /// <summary>
        /// Add a dummy cache entry, with file dependencies. 
        /// On this dummy cache expiracy, purge the _cacheProvider.
        /// </summary>
        /// <param name="cacheKey">Key to invalidate on file change</param>
        /// <param name="fileDependencies"></param>
        private void MonitorFileChanges(CacheKey cacheKey, IList<string> fileDependencies)
        {
            if (fileDependencies != null && fileDependencies.Any())
            {
                var monitoredFile = _virtualPathProvider.GetCacheDependency(fileDependencies.First(),
                    fileDependencies, DateTime.UtcNow);

                HostingEnvironment.Cache.Add(cacheKey.GetFullCacheKey(),
                    cacheKey,
                    monitoredFile,
                    Cache.NoAbsoluteExpiration,
                    Cache.NoSlidingExpiration,
                    CacheItemPriority.High,
                    (s, removedCacheKey, reason) =>
                    {
                        _cacheProvider.Remove((CacheKey)removedCacheKey);
                    });
            }
        }


        /// <summary>
        /// Gets the localized string based on the category, the key and the culture.
        /// </summary>
        /// <param name="param">The parameter which contains the category, the key and the culture.</param>
        /// <returns>
        /// Localized string
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// GetLocalizedParam.Category
        /// or
        /// GetLocalizedParam.Key
        /// or
        /// GetLocalizedParam.Culture
        /// </exception>
        public string GetLocalizedString(GetLocalizedParam param)
        {
            if (string.IsNullOrWhiteSpace(param.Category))
            {
                throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Category"), "param");
            }

            if (string.IsNullOrWhiteSpace(param.Key))
            {
                throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Key"), "param");
            }

            if (param.CultureInfo == null || string.IsNullOrWhiteSpace(param.CultureInfo.Name))
            {
                throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Culture"), "param");
            }

            return GetLocalizedStringAsync(param).Result;
        }

        /// <summary>
        /// Effectivly find the value for the requested Key,Category,Culture
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private async Task<string> GetLocalizedStringAsync(GetLocalizedParam param)
        {
            LocalizationCategory category;
            string value;

            LocalizationTree tree = await GetLocalizationTreeAsync(param.CultureInfo).ConfigureAwait(false);

            if (tree.LocalizedCategories.TryGetValue(param.Category.ToLowerInvariant(), out category)
             && category.LocalizedValues.TryGetValue(param.Key,      out value))
            {
                return value;
            }
            return null;
        }
    }
}
