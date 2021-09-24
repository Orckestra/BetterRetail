using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.ExperienceManagement.Configuration;
using System;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class ScopeProvider : IScopeProvider
    {
        private readonly Lazy<string> _lazyDefaultScope;
        public IWebsiteContext WebsiteContext;
        public ISiteConfiguration SiteConfiguration;
        protected IScopeViewService ScopeViewService { get; }

        public ScopeProvider(IWebsiteContext websiteContext, 
            ISiteConfiguration siteConfiguration,
            IScopeViewService scopeViewService
            )
        {
            _lazyDefaultScope = new Lazy<string>(GetDefaultScopeFromConfiguration, true);
            WebsiteContext = websiteContext;
            SiteConfiguration = siteConfiguration;
            ScopeViewService = scopeViewService;
        }

        private string GetDefaultScopeFromConfiguration()
        {
            return SiteConfiguration.GetScopeId(WebsiteContext.WebsiteId);
        }

        public Scope GetScopeById(string scopeId)
        {
            return ScopeViewService.GetScopeAsync(scopeId).Result;
        }

        public virtual string DefaultScope
        {
            get { return _lazyDefaultScope.Value; }
        }
    }
}
