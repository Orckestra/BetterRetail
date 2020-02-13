using Orckestra.Composer.Utils;
using System;
using System.IO;

namespace Orckestra.Composer.Sitemap.Config
{
    public class SitemapGeneratorConfig : ISitemapGeneratorConfig
    {
        private string _sitemapDirectory;
        private string _workingDirectory;

        public SitemapGeneratorConfig(string sitemapDirectory, string workingDirectory)
        {
            Guard.NotNullOrWhiteSpace(sitemapDirectory, nameof(sitemapDirectory));
            Guard.NotNullOrWhiteSpace(workingDirectory, nameof(workingDirectory));

            _sitemapDirectory = sitemapDirectory;
            _workingDirectory = workingDirectory;
        }

        public string GetSitemapDirectory(Guid website)
        {
            return Path.Combine( _sitemapDirectory, website.ToString());
        }

        public string GetWorkingDirectory(Guid website)
        {
            return Path.Combine(_workingDirectory, website.ToString());
        }

        public string GetWorkingRootDirectory()
        {
            return _workingDirectory;
        }
    }
}
