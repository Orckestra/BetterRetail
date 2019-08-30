using Orckestra.ExperienceManagement.Configuration;
using System;

namespace Orckestra.Composer.Providers
{
    public class ScopeProvider : IScopeProvider
    {
        private readonly Lazy<string> _lazyDefaultScope; 

        public ScopeProvider()
        {
            _lazyDefaultScope = new Lazy<string>(GetDefaultScopeFromConfiguration, true);
        }

        private string GetDefaultScopeFromConfiguration()
        {

            return SiteConfiguration.GetScopeId();
        }

        public string DefaultScope
        {
            get { return _lazyDefaultScope.Value; }
        }
    }
}
