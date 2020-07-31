using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.ExperienceManagement.Configuration;
using System;
using Composite.Data;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public class ScopeProvider : IScopeProvider
    {
        public IContextLanguageProvider ContextLanguageProvider { get; }
        private readonly Lazy<string> _lazyDefaultScope;
        public IWebsiteContext WebsiteContext;
        public ISiteConfiguration SiteConfiguration;

        public ScopeProvider(IWebsiteContext websiteContext, ISiteConfiguration siteConfiguration, IContextLanguageProvider contextLanguageProvider)
        {
            ContextLanguageProvider = contextLanguageProvider;
            _lazyDefaultScope = new Lazy<string>(GetDefaultScopeFromConfiguration, true);
            WebsiteContext = websiteContext;
            SiteConfiguration = siteConfiguration;
        }

        private string GetDefaultScopeFromConfiguration()
        {
            using (new DataScope(ContextLanguageProvider.GetCurrentCultureInfo()))
            {
                return SiteConfiguration.GetScopeId(WebsiteContext.WebsiteId);
            }
        }

        public virtual string DefaultScope
        {
            get { return _lazyDefaultScope.Value; }
        }
    }
}
