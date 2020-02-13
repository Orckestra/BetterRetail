using System;
using System.Web.Caching;

namespace Orckestra.Composer.OutputCache
{
    public class NullCacheProvider: OutputCacheProvider
    {
        public override object Get(string key)
        {
            return null;
        }

        public override object Add(string key, object entry, DateTime utcExpiry)
        {
            return null;
        }

        public override void Set(string key, object entry, DateTime utcExpiry)
        {
            //Nothing to do here.
        }

        public override void Remove(string key)
        {
            //Nothing to do here.
        }
    }
}
