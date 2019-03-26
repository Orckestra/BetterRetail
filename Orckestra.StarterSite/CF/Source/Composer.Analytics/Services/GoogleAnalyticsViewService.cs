using System;
using System.Configuration;
using Orckestra.Composer.GoogleAnalytics.ViewModels;

namespace Orckestra.Composer.GoogleAnalytics.Services
{
    public class GoogleAnalyticsViewService : IAnalyticsViewService
    {
        private const string ComposerGoogleTagManagerConfigKey = "Composer.GTMContainerId";

        public string GtmContainerId
        {
            get { return ConfigurationManager.AppSettings[ComposerGoogleTagManagerConfigKey]; }
        }

        public GoogleAnalyticsViewModel GetAnalyticsViewModel()
        {
            var analyticsViewModel = new GoogleAnalyticsViewModel()
            {
                ViewName = "GoogleAnalytics",
                TrackingId = GtmContainerId
            };

            return analyticsViewModel;
        }
    }
}