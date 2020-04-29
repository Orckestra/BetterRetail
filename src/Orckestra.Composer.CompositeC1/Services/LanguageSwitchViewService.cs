using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels.LanguageSwitch;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class LanguageSwitchViewService : ILanguageSwitchService
    {
        protected ICultureService CultureService { get; set; }

        public LanguageSwitchViewService(ICultureService cultureService)
        {
            CultureService = cultureService ?? throw new ArgumentNullException(nameof(cultureService));
        }

        public virtual LanguageSwitchViewModel GetViewModel(Func<CultureInfo, string> urlBuilder, CultureInfo currentCulture)
        {
            if (urlBuilder == null) { throw new ArgumentNullException(nameof(urlBuilder)); }
            if (currentCulture == null) { throw new ArgumentNullException(nameof(currentCulture)); }

            var supportedCultures = CultureService.GetAllSupportedCultures();

            var languageSwitchViewModel = new LanguageSwitchViewModel
            {
                IsMultiLanguage = supportedCultures.Length > 1
            };

            BuildMultiLanguageViewModel(languageSwitchViewModel, supportedCultures, urlBuilder, currentCulture);

            return languageSwitchViewModel;
        }

        protected virtual void BuildMultiLanguageViewModel(
            LanguageSwitchViewModel languageSwitchViewModel, 
            IEnumerable<CultureInfo> supportedCultures,
            Func<CultureInfo, string> urlBuilder, 
            CultureInfo currentCulture)
        {
            if (!languageSwitchViewModel.IsMultiLanguage) { return; }

            languageSwitchViewModel.CurrentLanguage = CreateEntry(urlBuilder, currentCulture, currentCulture);
            languageSwitchViewModel.Entries = CreateAlternativeEntries(supportedCultures, urlBuilder, currentCulture);

            languageSwitchViewModel.IsMultiLanguage = languageSwitchViewModel.Entries.Count > 1;
        }

        protected virtual List<LanguageSwitchEntryViewModel> CreateAlternativeEntries(
            IEnumerable<CultureInfo> supportedCultures, 
            Func<CultureInfo, string> urlBuilder, 
            CultureInfo currentCulture)
        {
            return supportedCultures
                .Select(ci => CreateEntry(urlBuilder, ci, currentCulture)).Where(e => e.Url != null).OrderBy(e => e.DisplayName).ToList();
        }

        protected virtual LanguageSwitchEntryViewModel CreateEntry(
            Func<CultureInfo, string> urlBuilder, 
            CultureInfo entryCulture, 
            CultureInfo currentCulture)
        {
            var url = urlBuilder.Invoke(entryCulture);

            var entry = new LanguageSwitchEntryViewModel
            {
                IsCurrent = entryCulture.Equals(currentCulture),
                Url = url
            };

            SetEntryDisplayNames(entry, entryCulture);

            return entry;
        }

        public virtual void SetEntryDisplayNames(LanguageSwitchEntryViewModel entry, CultureInfo entryCulture)
        {
            var displayName = (entryCulture.CultureTypes & CultureTypes.SpecificCultures) != 0 ? entryCulture.Parent.NativeName : entryCulture.NativeName;
            entry.DisplayName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(displayName);
            entry.ShortDisplayName = entryCulture.TwoLetterISOLanguageName;
            entry.CultureName = entryCulture.Name;
        }
    }
}