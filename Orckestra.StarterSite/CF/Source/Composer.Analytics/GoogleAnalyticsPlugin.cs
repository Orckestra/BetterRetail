using Orckestra.Composer.GoogleAnalytics.Services;

namespace Orckestra.Composer.GoogleAnalytics
{
    public class GoogleAnalyticsPlugin : IComposerPlugin
    {
        public void Register(IComposerHost host)
        {            
            host.MetadataRegistry.LoadViewModelMetadataInAssemblyOf(typeof (GoogleAnalyticsPlugin).Assembly);
            host.Register<GoogleAnalyticsViewService, IAnalyticsViewService>();
        }
    }
}