namespace Orckestra.Composer.CompositeC1.Sitemap
{
    public interface IC1SitemapConfiguration
    {
        bool IsScheduleDefined { get; }

        int ScheduleDelayInSeconds { get; }

        int NumberOfEntriesPerFile { get; }

        string SitemapDirectory { get; }

        string WorkingDirectory { get; }
    }
}
