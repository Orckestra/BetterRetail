namespace Orckestra.Composer.Sitemap.Config
{
    public interface ISitemapGeneratorConfig
    {
        string GetSitemapDirectory(SitemapParams sitemapParams);

        string GetWorkingDirectory(SitemapParams sitemapParams);

        string GetWorkingRootDirectory();
    }
}
