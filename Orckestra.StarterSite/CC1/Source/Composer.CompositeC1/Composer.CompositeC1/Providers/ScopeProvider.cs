using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Providers;
using Orckestra.ExperienceManagement.Configuration;

namespace Orckestra.Composer.CompositeC1.Providers
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
