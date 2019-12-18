using System;
using System.Globalization;
using System.Threading.Tasks;
using Orckestra.Composer.Services;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;


namespace Orckestra.Composer.CompositeC1.Services
{
    public class HomeViewService : IHomeViewService
    {
        protected ILocalizationProvider LocalizationProvider { get; }

        public HomeViewService(ILocalizationProvider localizationProvider)
        {
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
        }
     
        public virtual Task<string> GetCopyright(CultureInfo culture)
        {
            var getLocalizedCopyrightParam = new GetLocalizedParam
            {
                Category = "General",
                Key = "L_Copyright",
                CultureInfo = culture
            };

            var localizedCopyright = LocalizationProvider.GetLocalizedString(getLocalizedCopyrightParam);

            if (!string.IsNullOrWhiteSpace(localizedCopyright))
            {
                return Task.FromResult(string.Format(localizedCopyright, DateTime.Now.Year));
            }

            return Task.FromResult(string.Empty);
        }
    }
}
