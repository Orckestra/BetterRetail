using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.GoogleAnalytics.ViewModels
{
    public sealed class GoogleAnalyticsViewModel : BaseViewModel
    {
        public string ViewName { get; set; }
        public string TrackingId { get; set; }
    }
}