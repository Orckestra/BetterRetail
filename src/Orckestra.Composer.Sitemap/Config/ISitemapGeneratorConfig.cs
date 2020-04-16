using Orckestra.Composer.Utils;
using System;

namespace Orckestra.Composer.Sitemap.Config
{
    public interface ISitemapGeneratorConfig
    {
        string GetSitemapDirectory(Guid website);

        string GetWorkingDirectory(Guid website);

        string GetWorkingRootDirectory();
    }
}
