using System;
using System.Globalization;
using Orckestra.Composer.ViewModels.LanguageSwitch;

namespace Orckestra.Composer.Services
{
    public interface ILanguageSwitchService
    {
        /// <summary>
        /// Creates a ViewModel for Language Switch usage.
        /// </summary>
        /// <param name="urlBuilder">Method builder a URL for a given CultureInfo.</param>
        /// <param name="currentCulture">CultureInfo currently being used by the website.</param>
        /// <returns></returns>
        LanguageSwitchViewModel GetViewModel(Func<CultureInfo, string> urlBuilder, CultureInfo currentCulture);
    }
}
