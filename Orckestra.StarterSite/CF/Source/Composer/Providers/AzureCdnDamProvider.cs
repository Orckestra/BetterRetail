using System;

namespace Orckestra.Composer.Providers
{
    public class AzureCdnDamProvider : DamProviderBase
    {
        protected override string FileLocation
        {
            get { return string.Format("{0}://{1}", IsHttpsEnabled ? "https" : "http", ServerUrl);}
        }
    }
}
