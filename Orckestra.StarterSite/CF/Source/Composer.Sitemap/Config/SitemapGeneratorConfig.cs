using Orckestra.Composer.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Sitemap.Config
{
    public class SitemapGeneratorConfig : ISitemapGeneratorConfig
    {
        public SitemapGeneratorConfig(string sitemapDirectory, string sitemapIndexDirectory, string workingDirectory)
        {
            Guard.NotNullOrWhiteSpace(sitemapDirectory, nameof(sitemapDirectory));
            Guard.NotNullOrWhiteSpace(sitemapIndexDirectory, nameof(sitemapIndexDirectory));
            Guard.NotNullOrWhiteSpace(workingDirectory, nameof(workingDirectory));

            SitemapDirectory = sitemapDirectory;
            SitemapIndexDirectory = sitemapIndexDirectory;
            WorkingDirectory = workingDirectory;
        }

        public string SitemapDirectory { get; private set; }

        public string SitemapIndexDirectory { get; private set; }

        public string WorkingDirectory { get; private set; }
    }
}
