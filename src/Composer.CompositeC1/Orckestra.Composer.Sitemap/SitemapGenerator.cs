using Orckestra.Composer.Logging;
using Orckestra.Composer.Sitemap.Config;
using Orckestra.Composer.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Sitemap.Models;

namespace Orckestra.Composer.Sitemap
{
    public class SitemapGenerator : ISitemapGenerator
    {
        private static ILog Log = LogProvider.GetCurrentClassLogger();

        public static readonly string SitemapIndexFilename = "sitemap.xml";

        private static object _exclusiveLock = new object();

        private readonly IEnumerable<ISitemapProvider> _providers;
        private readonly ISitemapIndexGenerator _indexGenerator;
        private readonly ISitemapGeneratorConfig _config;

        public SitemapGenerator(IEnumerable<ISitemapProvider> providers, ISitemapIndexGenerator indexGenerator, ISitemapGeneratorConfig config)
        {
            Guard.NotNull(providers, nameof(providers));
            Guard.NotNull(indexGenerator, nameof(indexGenerator));
            Guard.NotNull(config, nameof(config));

            _providers = providers;
            _indexGenerator = indexGenerator;
            _config = config;
        }

        public void GenerateSitemaps(SitemapParams sitemapParams, string baseSitemapUrl, params CultureInfo[] cultures)
        {
            Log.Info("Starting sitemaps generation");            

            var stopwatch = Stopwatch.StartNew();

            Guard.NotNullOrWhiteSpace(sitemapParams.BaseUrl, nameof(sitemapParams.BaseUrl));            
            Guard.NotNullOrWhiteSpace(sitemapParams.Scope, nameof(sitemapParams.Scope));
            Guard.NotNullOrEmpty(cultures, nameof(cultures));

            lock (_exclusiveLock)
            {
                var sitemapDirectory = _config.GetSitemapDirectory(sitemapParams);
                EnsureDirectoryExists(sitemapDirectory);

                var tasks = new List<Task>();
                var sitemapNames = new List<string>();

                try
                {
                    EnsureDirectoryExists(_config.GetWorkingDirectory(sitemapParams));

                    foreach (var culture in cultures)
                    {
                        foreach (var provider in _providers)
                        {
                            // Start a new task for each provider. 
                            // For example we can generate content + product sitemaps at the same time.
                            tasks.Add(Task.Factory.StartNew(() =>
                            {
                                Log.Info($"Generating sitemap (type:{provider.GetType()}) for {culture.Name} in {sitemapParams.Scope} scope.");
                                foreach (var sitemap in provider.GenerateSitemaps(sitemapParams, culture: culture))
                                {
                                    // Write sitemap to disk
                                    Log.Info($"Writing sitemap {sitemap.Name}");
                                    WriteSitemap(sitemap, sitemapParams);

                                    // Add sitemap name to the list for the index creation later
                                    lock (sitemapNames)
                                    {
                                        sitemapNames.Add(sitemap.Name);
                                    }
                                }
                            }));
                        }
                    }

                    Task.WhenAll(tasks).Wait();

                    // Write sitemap index
                    var index = _indexGenerator.Generate(baseSitemapUrl, sitemapNames);
                    WriteSitemapIndex(index, sitemapParams);

                    // Deploy sitemaps and sitemap index
                    Log.Info($"Deploying sitemaps to {sitemapDirectory}");
                    DeploySitemaps(sitemapParams);
                }
                finally
                {
                    DeleteWorkingDirectory();
                }

                // Log stopwatch duration                 
                Log.Info($"Sitemaps generation completed. Generation took {stopwatch.Elapsed.TotalSeconds} seconds.");
            }
        }

        private void DeploySitemaps(SitemapParams sitemapParams)
        {
            var sitemapDirectory = _config.GetSitemapDirectory(sitemapParams);
            var destDirInfo = new DirectoryInfo(sitemapDirectory);
            var workingDirectory = _config.GetWorkingDirectory(sitemapParams);
            var tempDirInfo = new DirectoryInfo(workingDirectory);

            var sitemapIndexOriginFilepath = Path.Combine(workingDirectory, SitemapIndexFilename);
            var sitemapIndexDestinationFilepath = Path.Combine(sitemapDirectory, SitemapIndexFilename);

            // Delete sitemap index
            if (File.Exists(sitemapIndexDestinationFilepath))
            {
                File.Delete(sitemapIndexDestinationFilepath);
            }

            // Cleanup destination directory first
            var siteMapsToDelete = destDirInfo
                .GetFiles()
                .Where(fileInfo => _providers.Any(provider => provider.IsMatch(fileInfo.Name)));

            foreach (var siteMapToDelete in siteMapsToDelete)
            {
                siteMapToDelete.Delete();
            }

            // Move sitemap index
            File.Move(sitemapIndexOriginFilepath, sitemapIndexDestinationFilepath);

            // Move temporary sitemap files to destination directory
            // Include all directory content (index + sitemaps)
            foreach (var siteMapFileInfo in tempDirInfo.GetFiles())
            {
                var destinationFilePath = Path.Combine(sitemapDirectory, siteMapFileInfo.Name);
                siteMapFileInfo.MoveTo(destinationFilePath);
            }
        }

        private void EnsureDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        private void DeleteWorkingDirectory()
        {
            var workingDirectory = _config.GetWorkingRootDirectory();
            if (Directory.Exists(workingDirectory))
            {
                Directory.Delete(workingDirectory, recursive: true);
            }
        }

        private void WriteSitemap(Models.Sitemap sitemap, SitemapParams sitemapParams)
        {
            var filePath = Path.Combine(_config.GetWorkingDirectory(sitemapParams), sitemap.Name);
            sitemap.WriteToXml(filePath);
        }

        private void WriteSitemapIndex(SitemapIndex sitemapIndex, SitemapParams sitemapParams)
        {
            var filePath = Path.Combine(_config.GetWorkingDirectory(sitemapParams), SitemapIndexFilename);
            sitemapIndex.WriteToXml(filePath);
        }
    }
}
