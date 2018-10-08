using System;
using System.Configuration;
using Orckestra.Composer.Configuration;

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
            string scopeName = null;
            var section = ConfigurationManager.GetSection(ComposerConfigurationSection.ConfigurationName) as ComposerConfigurationSection;

            if (section != null)
            {
                var config = section.DefaultScope;

                if (config != null)
                {
                    scopeName = config.ScopeName;

                    if (string.IsNullOrWhiteSpace(scopeName)) { throw new ConfigurationErrorsException(string.Format("The value ('{0}') of the 'scopeName' property defined in the 'defaultScope' element in the 'composer' configuration is invalid.", scopeName ?? "null"));}
                }
            }

            return scopeName ?? "Global";
        }

        public string DefaultScope
        {
            get { return _lazyDefaultScope.Value; }
        }
    }
}
