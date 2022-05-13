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
            _sitemapDirectory = sitemapDirectory ?? throw new ArgumentNullException(nameof(sitemapDirectory));
            _workingDirectory = workingDirectory ?? throw new ArgumentNullException(nameof(workingDirectory));
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
