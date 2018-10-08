using Orckestra.Composer.Logging;
using Orckestra.Composer.Sitemap.Config;
using Orckestra.Composer.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Orckestra.Composer.Sitemap.Models;

namespace Orckestra.Composer.Sitemap
{
    public class SitemapGenerator : ISitemapGenerator
    {
        private static ILog Log = LogProvider.GetCurrentClassLogger();

        public const string SitemapIndexFilename = "sitemap.xml";

        private static object _exclusiveLock = new object();

        private readonly IEnumerable<ISitemapProvider> _providers;
        private readonly ISitemapIndexGenerator _indexGenerator;
        private readonly string _sitemapDirectory;
        private readonly string _sitemapIndexDirectory;
        private readonly string _workingDirectory;

        public SitemapGenerator(IEnumerable<ISitemapProvider> providers, ISitemapIndexGenerator indexGenerator, ISitemapGeneratorConfig config)
        {
            Guard.NotNull(providers, nameof(providers));
            Guard.NotNull(indexGenerator, nameof(indexGenerator));
            Guard.NotNull(config, nameof(config));
            Guard.NotNullOrWhiteSpace(config.SitemapDirectory, nameof(config.SitemapDirectory));
            Guard.NotNullOrWhiteSpace(config.SitemapIndexDirectory, nameof(config.SitemapIndexDirectory));
            Guard.NotNullOrWhiteSpace(config.WorkingDirectory, nameof(config.WorkingDirectory));

            _providers = providers;
            _indexGenerator = indexGenerator;
            _sitemapDirectory = config.SitemapDirectory;
            _sitemapIndexDirectory = config.SitemapIndexDirectory;
            _workingDirectory = config.WorkingDirectory;
        }

        public void GenerateSitemaps(string baseUrl, string baseSitemapUrl, string scope, params CultureInfo[] cultures)
        {
            Log.Info("Starting sitemaps generation");            

            var stopwatch = Stopwatch.StartNew();

            Guard.NotNullOrWhiteSpace(baseUrl, nameof(baseUrl));            
            Guard.NotNullOrWhiteSpace(scope, nameof(scope));
            Guard.NotNullOrEmpty(cultures, nameof(cultures));

            lock (_exclusiveLock)
            {
                EnsureDirectoryExists(_sitemapDirectory);
                EnsureDirectoryExists(_sitemapIndexDirectory);

                var tasks = new List<Task>();
                var sitemapNames = new List<string>();

                try
                {
                    EnsureDirectoryExists(_workingDirectory);

                    foreach (var culture in cultures)
                    {
                        foreach (var provider in _providers)
                        {
                            // Start a new task for each provider. 
                            // For example we can generate content + product sitemaps at the same time.
                            tasks.Add(Task.Factory.StartNew(() =>
                            {
                                Log.Info($"Generating sitemap (type:{provider.Namer.GetType()}) for {culture.Name} in {scope} scope.");
                                foreach (var sitemap in provider.GenerateSitemaps(baseUrl: baseUrl, scope: scope, culture: culture))
                                {
                                    // Write sitemap to disk
                                    Log.Info($"Writing sitemap {sitemap.Name}");
                                    WriteSitemap(sitemap);

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
                    WriteSitemapIndex(index);

                    // Deploy sitemaps and sitemap index
                    Log.Info($"Deploying sitemaps to {_sitemapDirectory}");
                    DeploySitemaps();
                }
                finally
                {
                    DeleteWorkingDirectory();
                }

                // Log stopwatch duration                 
                Log.Info($"Sitemaps generation completed. Generation took {stopwatch.Elapsed.TotalSeconds} seconds.");
            }
        }

        private void DeploySitemaps()
        {
            var destDirInfo = new DirectoryInfo(_sitemapDirectory);
            var tempDirInfo = new DirectoryInfo(_workingDirectory);

            var sitemapIndexOriginFilepath = Path.Combine(_workingDirectory, SitemapIndexFilename);
            var sitemapIndexDestinationFilepath = Path.Combine(_sitemapIndexDirectory, SitemapIndexFilename);

            // Delete sitemap index
            if (File.Exists(sitemapIndexDestinationFilepath))
            {
                Log.Info($"Deleting sitemap index: {sitemapIndexDestinationFilepath}");
                File.Delete(sitemapIndexDestinationFilepath);
            }

            // Cleanup destination directory first
            var namers = _providers.Select(provider => provider.Namer);

            var siteMapsToDelete = destDirInfo
                .GetFiles()
                .Where(fileInfo => namers.Any(namer => namer.IsMatch(fileInfo.Name)));

            foreach (var siteMapToDelete in siteMapsToDelete)
            {
                Log.Info($"Deleting sitemap: {siteMapToDelete}");
                siteMapToDelete.Delete();
            }

            // Move sitemap index
            File.Move(sitemapIndexOriginFilepath, sitemapIndexDestinationFilepath);

            // Move temporary sitemap files to destination directory
            // Include all directory content (index + sitemaps)
            foreach (var siteMapFileInfo in tempDirInfo.GetFiles())
            {
                var destinationFilePath = Path.Combine(_sitemapDirectory, siteMapFileInfo.Name);
                Log.Info($"Moving sitemap file: {siteMapFileInfo.Name} to {_sitemapDirectory} ({siteMapFileInfo.FullName} => {destinationFilePath})");
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
            if (Directory.Exists(_workingDirectory))
            {
                Directory.Delete(_workingDirectory, recursive: true);
            }
        }

        private void WriteSitemap(Models.Sitemap sitemap)
        {
            var filePath = Path.Combine(_workingDirectory, sitemap.Name);
            sitemap.WriteToXml(filePath);
        }

        private void WriteSitemapIndex(SitemapIndex sitemapIndex)
        {
            var filePath = Path.Combine(_workingDirectory, SitemapIndexFilename);
            sitemapIndex.WriteToXml(filePath);
        }
    }
}
