using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.ExperienceManagement.Configuration;
using System;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class ScopeProvider : IScopeProvider
    {
        private readonly Lazy<string> _lazyDefaultScope;
        public IWebsiteContext WebsiteContext;
        public ISiteConfiguration SiteConfiguration;

        public ScopeProvider(IWebsiteContext websiteContext, 
            ISiteConfiguration siteConfiguration
            )
        {
            _lazyDefaultScope = new Lazy<string>(GetDefaultScopeFromConfiguration, true);
            WebsiteContext = websiteContext;
            SiteConfiguration = siteConfiguration;
        }

        private string GetDefaultScopeFromConfiguration()
        {
            return SiteConfiguration.GetScopeId(WebsiteContext.WebsiteId);
        }

        public virtual string DefaultScope
        {
            get { return _lazyDefaultScope.Value; }
        }
    }
}
