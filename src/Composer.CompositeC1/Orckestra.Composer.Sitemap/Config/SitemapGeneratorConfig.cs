using Orckestra.Composer.Utils;
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

        public string GetSitemapDirectory(SitemapParams sitemapParams)
        {
            return Path.Combine( _sitemapDirectory, sitemapParams.Website.ToString());
        }

        public string GetWorkingDirectory(SitemapParams sitemapParams)
        {
            return Path.Combine(_workingDirectory, sitemapParams.Website.ToString());
        }

        public string GetWorkingRootDirectory()
        {
            return _workingDirectory;
        }
    }
}
