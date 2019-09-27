using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.ExperienceManagement.Configuration;
using System;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class CountryCodeProvider : ICountryCodeProvider
    {
        private readonly Lazy<string> _lazyCountryCode;
        public IWebsiteContext WebsiteContext;
        public ISiteConfiguration SiteConfiguration;

        public CountryCodeProvider(IWebsiteContext websiteContext, ISiteConfiguration siteConfiguration)
        {
            _lazyCountryCode = new Lazy<string>(GetCountryCodeFromConfiguration, true);
            WebsiteContext = websiteContext;
            SiteConfiguration = siteConfiguration;
        }

        private string GetCountryCodeFromConfiguration()
        {
            return SiteConfiguration.GetCountryCode(WebsiteContext.WebsiteId);
        }

        public string CountryCode
        {
            get { return _lazyCountryCode.Value; }
        }
    }
}
