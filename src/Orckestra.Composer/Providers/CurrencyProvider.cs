using Orckestra.Composer.Services;
using System;

namespace Orckestra.Composer.Providers
{
    public class CurrencyProvider: ICurrencyProvider
    {
        protected IComposerContext ComposerContext { get; private set; }

        public CurrencyProvider(IComposerContext composerContext)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
        }

        public virtual string GetCurrency()
        {
            return ComposerContext.ScopeCurrencyIso;
        }
    }
}
