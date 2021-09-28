using System;

namespace Orckestra.Composer.Services
{
    public class CurrencyConversionSettingsService: ICurrencyConversionSettingsService
    {
        protected IComposerContext ComposerContext { get; private set; }

        public CurrencyConversionSettingsService(IComposerContext composerContext)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
        }

        public virtual string GetScopeCurrency()
        {
            return ComposerContext.CurrencyIso;
        }
    }
}
