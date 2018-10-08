using Orckestra.Composer.GoogleAnalytics.ViewModels;

namespace Orckestra.Composer.GoogleAnalytics.Services
{
    public interface IAnalyticsViewService
    {
        GoogleAnalyticsViewModel GetAnalyticsViewModel();
    }
}