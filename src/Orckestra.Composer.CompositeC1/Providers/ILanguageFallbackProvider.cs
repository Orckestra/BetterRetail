using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.CompositeC1.Providers
{
    public interface ILanguageFallbackProvider
    {
        IDisposable GetInvariantLanguageScope();

    }

    internal class LanguageFallbackProvider : ILanguageFallbackProvider
    {
        public IDisposable GetInvariantLanguageScope()
        {
            return new InvariantScope();
        }

        private class InvariantScope : IDisposable
        {
            private static readonly string InvariantLanguageFallbackCountCacheKey = "InvariantLanguageFallback";
            public InvariantScope()
            {
                var current = CallContext.GetData(InvariantLanguageFallbackCountCacheKey) as int? ?? 0;
                CallContext.SetData(InvariantLanguageFallbackCountCacheKey, current + 1);
            }

            public void Dispose()
            {
                var current = CallContext.GetData(InvariantLanguageFallbackCountCacheKey) as int? ?? 0;
                CallContext.SetData(InvariantLanguageFallbackCountCacheKey, current - 1);
            }
        }
    }
}
