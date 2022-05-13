using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Xml;
using Orckestra.Composer.Kernel;
using Orckestra.Composer.TypeExtensions;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Providers.Localization
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ResxLocalizationRepository : ILocalizationRepository<VirtualFile>
    {
        private readonly Regex  _regexSource;
        private readonly VirtualPathProvider _virtualPathProvider;

        public ResxLocalizationRepository(IComposerEnvironment environment)
        {
            if (environment == null) { throw new ArgumentNullException(nameof(environment)); }

            _virtualPathProvider = environment.VirtualPathProvider;

           _regexSource = new Regex(ComposerConfiguration.ResxLocalizationRepositoryConfiguration.RegexSource,
                                     RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

           EnsureResxDirectoryExists(ComposerConfiguration.ResxLocalizationRepositoryConfiguration.LocalizationResxDirectory);
        }

        /// <summary>
        /// Ensures the RESX directory exists.
        /// </summary>
        /// <param name="resxDirectoryName">Name of the RESX directory.</param>
        /// <exception cref="System.ArgumentNullException">resxDirectoryName</exception>
        /// <exception cref="System.ArgumentException">The resource directory  + resxDirectoryName +  does not exists.</exception>
        private void EnsureResxDirectoryExists(string resxDirectoryName)
        {
            if (string.IsNullOrEmpty(resxDirectoryName)) { throw new ArgumentException(GetMessageOfNullEmpty(), nameof(resxDirectoryName)); }

            if (!_virtualPathProvider.DirectoryExists(resxDirectoryName))
            {
                throw new ArgumentException("The resource directory " + resxDirectoryName + " does not exists.");
            }
        }

        /// <summary>
        /// Get all categories used to bundle localization
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllCategories()
        {
            var allFiles = RecursivelyFindAllResxFiles();

            List<string> allCategories = 
                allFiles.Select(FindCategoryFromFilename)
                        .Where(category => !string.IsNullOrEmpty(category))
                        .Distinct()
                        .ToList();

            return allCategories;
        }

        /// <summary>
        /// Extract the category part from the Filename,
        /// </summary>
        /// <param name="file"></param>
        /// <returns>CategoryName or Empty if none found</returns>
        private string FindCategoryFromFilename(VirtualFile file)
        {
            Match m = _regexSource.Match(file.Name ?? string.Empty);

            var value = m.Success 
                ? m.Groups["category"].Value 
                : string.Empty;

            return value;
        }

        /// <summary>
        /// Get all possible sources of localization for a given (key, category, culture).
        /// 
        /// The resulting list of sources is sorted with most significant source first
        /// the first value found for a given (key, category, culture) should be the exposed one.
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="culture"></param>
        /// <returns>Sorted List of resx FullFilenames to find keys for the (Category,Culture) tuple</returns>
        public List<VirtualFile> GetPrioritizedSources(string categoryName, CultureInfo culture)
        {
            var allFiles = RecursivelyFindAllResxFiles();
            var resolvedNames = ResolveConfiguredFilenamePatterns(categoryName, culture);

            List<VirtualFile> sources =
                resolvedNames.Join(allFiles, 
                    name => name, 
                    fileInfo => fileInfo.Name, 
                    (name,fileInfo) => fileInfo, StringComparer.InvariantCultureIgnoreCase)
                .ToList();

            return sources;
        }

        /// <summary>
        /// Create a Sorted List of filenames based on the Configured patterned
        /// 
        /// 
        /// Configured order is conserved and is usued to build up the sorting weights
        /// The sorting weight is only required to preserve configured order when joining on all files
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        private List<string> ResolveConfiguredFilenamePatterns(string categoryName, CultureInfo culture)
            {
            List<string> resolvedList =
                ComposerConfiguration.ResxLocalizationRepositoryConfiguration
                    .PatternsForPossibleSources
                    .Select(pattern => pattern.Replace("{category}", categoryName)
                                              .Replace("{cultureName}", culture.Name)
                                              .Replace("{twoLetterISOLanguageName}", culture.TwoLetterISOLanguageName)
                    ).ToList();

            return resolvedList;
        }

        /// <summary>
        /// Find all managed Resx Files
        /// </summary>
        /// <returns></returns>
        private IEnumerable<VirtualFile> RecursivelyFindAllResxFiles()
        {
            var resxDirectoryName =
                ComposerConfiguration.ResxLocalizationRepositoryConfiguration.LocalizationResxDirectory;

            var rootDirectory = _virtualPathProvider.GetDirectory(resxDirectoryName);
            return rootDirectory
                .Children
                .OfType<VirtualFile>()
                .Where(x => x.VirtualPath.EndsWith(".resx", StringComparison.InvariantCultureIgnoreCase));

        }

        /// <summary>
        /// Parses the given Resx file to extract all Key Value pairs
        /// </summary>
        /// <param name="source">The filename to a single resx file</param>
        /// <returns>Dictionary Fully populated with the localized string found in the resx</returns>
        public async Task<Dictionary<string, string>> GetValuesAsync(VirtualFile source)
        {
            Dictionary<string,string> values = new Dictionary<string, string>();

            using (Stream stream = source.Open())
            {
                XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings { Async = true });

                //Initiate the marker at the first data element;
                while (await reader.ReadToFollowingAsync("data", string.Empty).ConfigureAwait(false))
                {
                    string key = reader.GetAttribute("name");
                    if (!string.IsNullOrEmpty(key) &&
                     await reader.ReadToDescendantAsync("value", string.Empty).ConfigureAwait(false))
                    {
                        await reader.MoveToContentAsync().ConfigureAwait(false);
                        string value = reader.ReadString();

                        values[key] = value;
                    }
                }

                stream.Close();
            }

            return values;
        }
    }
}
