using Orckestra.Composer.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap.Config
{
    public class SitemapGeneratorConfig : ISitemapGeneratorConfig
    {
        private string _sitemapDirectory;
        private string _sitemapIndexDirectory;
        private string _workingDirectory;

        public SitemapGeneratorConfig(string sitemapDirectory, string sitemapIndexDirectory, string workingDirectory)
        {
            Guard.NotNullOrWhiteSpace(sitemapDirectory, nameof(sitemapDirectory));
            Guard.NotNullOrWhiteSpace(sitemapIndexDirectory, nameof(sitemapIndexDirectory));
            Guard.NotNullOrWhiteSpace(workingDirectory, nameof(workingDirectory));

            _sitemapDirectory = sitemapDirectory;
            _sitemapIndexDirectory = sitemapIndexDirectory;
            _workingDirectory = workingDirectory;
        }

        public string GetSitemapDirectory(SitemapParams sitemapParams)
        {
            return Path.Combine( _sitemapDirectory, sitemapParams.Website.ToString());
        }

        public string GetSitemapIndexDirectory(SitemapParams sitemapParams)
        {
            return Path.Combine(_sitemapIndexDirectory, sitemapParams.Website.ToString());
        }

        public string GetWorkingDirectory(SitemapParams sitemapParams)
        {
            return Path.Combine(_workingDirectory, sitemapParams.Website.ToString());
        }
    }
}
