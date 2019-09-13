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

        public CountryCodeProvider(IWebsiteContext websiteContext)
        {
            _lazyCountryCode = new Lazy<string>(GetCountryCodeFromConfiguration, true);
            WebsiteContext = websiteContext;
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
