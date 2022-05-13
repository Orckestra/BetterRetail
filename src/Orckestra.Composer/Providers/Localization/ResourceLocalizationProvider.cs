using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Hosting;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Kernel;
using Orckestra.Overture.Caching;

using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Providers.Localization
{
    /// <summary>
    /// This object 
    /// </summary>
    internal class ResourceLocalizationProvider : ILocalizationProvider
    {
        protected ICacheProvider CacheProvider { get; }
        protected ILocalizationRepository<VirtualFile> LocalizationRepository { get; }
        protected VirtualPathProvider VirtualPathProvider { get; }
        private Dictionary<CacheKey, FileSystemWatcher> fileWacherCollection = new Dictionary<CacheKey, FileSystemWatcher>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceLocalizationProvider"/> class.
        /// </summary>
        public ResourceLocalizationProvider(IComposerEnvironment environment, ICacheProvider cacheProvider)
        {
            if (environment == null) { throw new ArgumentNullException(nameof(environment)); }

            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            VirtualPathProvider = environment.VirtualPathProvider;

            LocalizationRepository = new ResxLocalizationRepository(environment);
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
            if (culture == null) { throw new ArgumentNullException(nameof(culture)); }

            CacheKey localizationTreeCacheKey = new CacheKey(CacheConfigurationCategoryNames.LocalizationTree)
            {
                CultureInfo = culture,
            };

            var value = CacheProvider.GetOrAddAsync(localizationTreeCacheKey, async () =>
            {
                string folderPhysicalPath = string.Empty;
                LocalizationTree tree = new LocalizationTree(culture);

                foreach (var categoryName in LocalizationRepository.GetAllCategories())
                {
                    LocalizationCategory category = new LocalizationCategory(categoryName);

                    foreach (var source in LocalizationRepository.GetPrioritizedSources(categoryName, culture))
                    {
                        Dictionary<string, string> values =
                            await LocalizationRepository.GetValuesAsync(source).ConfigureAwait(false);
                        foreach (var kvp in values)
                        {
                            if (!category.LocalizedValues.ContainsKey(kvp.Key))
                            {
                                category.LocalizedValues.Add(kvp.Key, kvp.Value);
                            }
                        }

                        Regex meinReg = new Regex("^[a-zA-Z]{1}:\\.*");
                        if (string.IsNullOrEmpty(folderPhysicalPath) && !meinReg.IsMatch(source.VirtualPath))
                        {
                            folderPhysicalPath = HostingEnvironment.MapPath(source.VirtualPath.Substring(0, source.VirtualPath.LastIndexOf(source.Name)));
                        }
                    }

                    tree.LocalizedCategories.Add(categoryName.ToLowerInvariant(), category);
                }

                MonitorLocalizationFiles(localizationTreeCacheKey, folderPhysicalPath);
                return tree;
            });

            return value;
        }

        /// <summary>
        /// Remove cache key if some files was changed 
        /// </summary>
        /// <param name="cacheKey">Key to invalidate on file change</param>
        /// <param name="directory">watch directory</param>
        private void MonitorLocalizationFiles(CacheKey cacheKey, string directory)
        {
            string fileWatcherMask = "*.resx";

            if (!fileWacherCollection.ContainsKey(cacheKey) && !string.IsNullOrEmpty(directory))
            {
                var fileSystemWatcher = new FileSystemWatcher(directory, fileWatcherMask);

                Action OnFileChanged = () => CacheProvider.Remove(cacheKey);

                fileSystemWatcher.Created += (a, b) => OnFileChanged();
                fileSystemWatcher.Changed += (a, b) => OnFileChanged();
                fileSystemWatcher.Deleted += (a, b) => OnFileChanged();
                fileSystemWatcher.Renamed += (a, b) => OnFileChanged();
                fileSystemWatcher.EnableRaisingEvents = true;

                fileWacherCollection.Add(cacheKey, fileSystemWatcher);
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
            if (string.IsNullOrWhiteSpace(param.Category)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Category)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Key)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Key)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CultureInfo?.Name)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CultureInfo.Name)), nameof(param)); }

            return GetLocalizedStringAsync(param).Result;
        }

        /// <summary>
        /// Effectivly find the value for the requested Key,Category,Culture
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private async Task<string> GetLocalizedStringAsync(GetLocalizedParam param)
        {
            LocalizationTree tree = await GetLocalizationTreeAsync(param.CultureInfo).ConfigureAwait(false);

            if (tree.LocalizedCategories.TryGetValue(param.Category.ToLowerInvariant(), out LocalizationCategory category)
             && category.LocalizedValues.TryGetValue(param.Key, out string value))
            {
                return value;
            }
            return null;
        }
    }
}