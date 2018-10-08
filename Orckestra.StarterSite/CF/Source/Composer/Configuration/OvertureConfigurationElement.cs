using System.Configuration;
using System.Net.Cache;
using Orckestra.Overture;
using Orckestra.Overture.RestClient;

namespace Orckestra.Composer.Configuration
{
    public class OvertureConfigurationElement : ConfigurationElement
    {
        public const string ConfigurationName = "overture";

        const string AuthTokenKey = "authToken";
        [ConfigurationProperty(AuthTokenKey, IsRequired = true)]
        public string AuthToken
        {
            get { return (string)this[AuthTokenKey]; }
            set { this[AuthTokenKey] = value; }
        }

        const string FormatKey = "format";
        [ConfigurationProperty(FormatKey, DefaultValue = ClientFormat.Json)]
        public ClientFormat Format
        {
            get { return (ClientFormat) this[FormatKey]; }
            set { this[FormatKey] = value; }
        }

        const string UrlKey = "url";
        [ConfigurationProperty(UrlKey, IsRequired = true, IsKey = true)]
        public string Url
        {
            get { return (string)this[UrlKey]; }
            set { this[UrlKey] = value; }
        }

        const string CacheLevelKey = "cache";
        [ConfigurationProperty(CacheLevelKey, DefaultValue = HttpRequestCacheLevel.Default)]
        public HttpRequestCacheLevel CacheLevel
        {
            get { return (HttpRequestCacheLevel) this[CacheLevelKey]; }
            set { this[CacheLevelKey] = value; }
        }
    }
}
