using System.Configuration;

namespace Orckestra.Composer.Configuration
{
    public class DefaultScopeConfigurationElement : ConfigurationElement
    {
        public const string ConfigurationName = "defaultScopeProvider";

        /// <summary>
        /// Determines the name of the scope for the Default Scope Provider.
        /// </summary>
        [ConfigurationProperty(ScopeNameKey, IsRequired = true, DefaultValue = "Global")]
        public string ScopeName
        {
            get { return (string)this[ScopeNameKey]; }
            set { this[ScopeNameKey] = value; }
        }
        private const string ScopeNameKey = "scopeName";
    }
}
