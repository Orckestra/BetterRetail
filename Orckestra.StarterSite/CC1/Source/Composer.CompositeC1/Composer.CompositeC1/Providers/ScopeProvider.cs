using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.ExperienceManagement.Configuration;
using System;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class ScopeProvider : IScopeProvider
    {
        private readonly Lazy<string> _lazyDefaultScope;
        public IWebsiteContext WebsiteContext;

        public ScopeProvider(IWebsiteContext websiteContext)
        {
            _lazyDefaultScope = new Lazy<string>(GetDefaultScopeFromConfiguration, true);
            WebsiteContext = websiteContext;
        }

        private string GetDefaultScopeFromConfiguration()
        {
            return SiteConfiguration.GetScopeId(WebsiteContext.WebsiteId);
        }

        public string DefaultScope
        {
            get { return _lazyDefaultScope.Value; }
        }
    }
}
