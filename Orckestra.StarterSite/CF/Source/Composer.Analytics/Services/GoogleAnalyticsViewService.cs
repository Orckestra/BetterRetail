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
            if (string.IsNullOrEmpty(GtmContainerId))
            {
                throw new Exception(string.Format("The configuration '{0}' cannot be null when using Google Analytics", ComposerGoogleTagManagerConfigKey));
            }

            var analyticsViewModel = new GoogleAnalyticsViewModel()
            {
                ViewName = "GoogleAnalytics",
                TrackingId = GtmContainerId
            };

            return analyticsViewModel;
        }
    }
}