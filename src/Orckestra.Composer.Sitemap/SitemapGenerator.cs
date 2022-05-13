using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Logging;
using Orckestra.Composer.Sitemap.Config;
using Orckestra.Composer.Sitemap.Models;
using Orckestra.ExperienceManagement.Configuration;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Sitemap
{
    public class SitemapGenerator : ISitemapGenerator
    {
        private static ILog Log = LogProvider.GetCurrentClassLogger();

        public static readonly string SitemapIndexFilename = "sitemap.xml";

        private static object _exclusiveLock = new object();

        private IEnumerable<ISitemapProvider> Providers { get; }
        private ISitemapIndexGenerator IndexGenerator { get; }
        private ISitemapGeneratorConfig Config { get; }
        private ISiteConfiguration SiteConfiguration { get; }

        public SitemapGenerator(IEnumerable<ISitemapProvider> providers, ISitemapIndexGenerator indexGenerator, ISitemapGeneratorConfig config, ISiteConfiguration siteConfiguration)
        {
            Providers = providers ?? throw new ArgumentNullException(nameof(ISitemapProvider));
            IndexGenerator = indexGenerator ?? throw new ArgumentNullException(nameof(ISitemapIndexGenerator));
            Config = config ?? throw new ArgumentNullException(nameof(ISitemapGeneratorConfig));
            SiteConfiguration = siteConfiguration ?? throw new ArgumentNullException(nameof(siteConfiguration));
        }

        public void GenerateSitemaps(Guid website, string baseUrl, string baseSitemapUrl, params CultureInfo[] cultures)
        {
            Log.Info("Starting sitemaps generation");

            var stopwatch = Stopwatch.StartNew();

            if (string.IsNullOrWhiteSpace(baseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(), nameof(baseUrl)); }
            if (cultures == null || cultures.Length == 0) { throw new ArgumentException(GetMessageOfNullEmpty(), nameof(cultures)); }
            if (website == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(), nameof(website)); }

            lock (_exclusiveLock)
            {
                var sitemapDirectory = Config.GetSitemapDirectory(website);
                EnsureDirectoryExists(sitemapDirectory);

                var tasks = new List<Task>();
                var sitemapNames = new List<string>();

                try
                {
                    EnsureDirectoryExists(Config.GetWorkingDirectory(website));

                    foreach (var culture in cultures)
                    {
                        try
                        {
                            string scope = SiteConfiguration.GetPublishedScopeId(culture, website);

                            foreach (var provider in Providers)
                            {
                                // Start a new task for each provider. 
                                // For example we can generate content + product sitemaps at the same time.
                                tasks.Add(Task.Factory.StartNew(() =>
                                {
                                    Log.Info($"Generating sitemap (type:{provider.GetType()}) for {culture.Name} in {scope} scope.");
                                    var sitemapParams = new SitemapParams
                                    {
                                        Website = website,
                                        BaseUrl = baseUrl,
                                        Scope = scope,
                                        Culture = culture
                                    };

                                    try
                                    {
                                        foreach (var sitemap in provider.GenerateSitemaps(sitemapParams))
                                        {
                                            // Write sitemap to disk
                                            Log.Info($"Writing sitemap {sitemap.Name}");
                                            WriteSitemap(sitemap, website);

                                            // Add sitemap name to the list for the index creation later
                                            lock (sitemapNames)
                                            {
                                                sitemapNames.Add(sitemap.Name);
                                            }
                                        }
                                    }
                                    catch (Exception e) 
                                    {
                                        Log.Error(e.ToString());
                                    }

                                }));
                            }
                        }
                        //TODO: process exeption
                        catch (ArgumentException) { }
                    }

                    Task.WhenAll(tasks).Wait();

                    // Write sitemap index
                    var index = IndexGenerator.Generate(baseSitemapUrl, sitemapNames);
                    WriteSitemapIndex(index, website);

                    // Deploy sitemaps and sitemap index
                    Log.Info($"Deploying sitemaps to {sitemapDirectory}");
                    DeploySitemaps(website);
                }
                finally
                {
                    DeleteWorkingDirectory();
                }

                // Log stopwatch duration                 
                Log.Info($"Sitemaps generation completed. Generation took {stopwatch.Elapsed.TotalSeconds} seconds.");
            }
        }

        private void DeploySitemaps(Guid website)
        {
            var sitemapDirectory = Config.GetSitemapDirectory(website);
            var destDirInfo = new DirectoryInfo(sitemapDirectory);
            var workingDirectory = Config.GetWorkingDirectory(website);
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
                .Where(fileInfo => Providers.Any(provider => provider.IsMatch(fileInfo.Name)));

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
            var workingDirectory = Config.GetWorkingRootDirectory();
            if (Directory.Exists(workingDirectory))
            {
                Directory.Delete(workingDirectory, recursive: true);
            }
        }

        private void WriteSitemap(Models.Sitemap sitemap, Guid website)
        {
            var filePath = Path.Combine(Config.GetWorkingDirectory(website), sitemap.Name);
            sitemap.WriteToXml(filePath);
        }

        private void WriteSitemapIndex(SitemapIndex sitemapIndex, Guid website)
        {
            var filePath = Path.Combine(Config.GetWorkingDirectory(website), SitemapIndexFilename);
            sitemapIndex.WriteToXml(filePath);
        }
    }
}
