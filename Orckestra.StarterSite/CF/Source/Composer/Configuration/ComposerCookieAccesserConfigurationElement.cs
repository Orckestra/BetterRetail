using System.Configuration;

namespace Orckestra.Composer.Configuration
{
    public class ComposerCookieAccesserConfigurationElement : ConfigurationElement
    {
        public const string ConfigurationName = "composerCookieAccesser";

        /// <summary>
        /// Optional attribute.
        ///
        /// Specifies the HTTP cookie to use for storing the payload.
        /// If multiple applications are running on a single server and each application requires a unique cookie,
        /// you must configure the cookie name in each Web.config file for each application.
        ///
        /// The default is ".Composer".
        /// </summary>
        [ConfigurationProperty(NameKey, DefaultValue = ".Composer", IsRequired = false)]
        public string Name
        {
            get { return (string)this[NameKey]; }
            set { this[NameKey] = value; }
        }
        const string NameKey = "name";

        /// <summary>
        /// Optional attribute.
        /// The Domain of the cookie.
        /// If you change cookie domain, also change the cookie name, so that everything will continue to work as expected
        ///
        /// The default is an empty string ("")
        /// </summary>
        [ConfigurationProperty(DomainKey, DefaultValue = "", IsRequired = false)]
        public string Domain
        {
            get { return (string)this[DomainKey]; }
            set { this[DomainKey] = value; }
        }
        const string DomainKey = "domain";

        /// <summary>
        /// Optional attribute.
        ///
        /// Specifies the time, in integer minutes, after which the cookie expires.
        /// To prevent compromised performance, and to avoid multiple browser warnings
        /// for users who have cookie warnings turned on, the timer is updated only when the payload changes.
        ///
        /// The default is "131760" (~3 months). 60*24*(30.5)*3
        /// </summary>
        [ConfigurationProperty(TimeoutInMinutesKey, DefaultValue = "131760", IsRequired = false)]
        public int Timeout
        {
            get { return (int)this[TimeoutInMinutesKey]; }
            set { this[TimeoutInMinutesKey] = value; }
        }
        const string TimeoutInMinutesKey = "timeoutInMinutes";

        /// <summary>
        /// Optional attribute.
        ///
        /// Specifies whether an SSL connection is required to transmit the payload cookie.
        /// When True; sets the Secure property for the Cookie and a compliant browser
        /// does not return the cookie, unless the connection is using SSL.
        ///
        /// The default is "True" (SSL is required).
        /// </summary>
        [ConfigurationProperty(RequireSslKey, DefaultValue = "True", IsRequired = false)]
        public bool RequireSsl
        {
            get { return (bool)this[RequireSslKey]; }
        }
        const string RequireSslKey = "requireSSL";
    }
}
