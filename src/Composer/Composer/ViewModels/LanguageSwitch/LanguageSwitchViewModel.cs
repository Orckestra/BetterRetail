using System.Collections.Generic;

namespace Orckestra.Composer.ViewModels.LanguageSwitch
{
    public sealed class LanguageSwitchViewModel : BaseViewModel
    {
        public bool IsMultiLanguage { get; set; }

        public LanguageSwitchEntryViewModel CurrentLanguage { get; set; }

        public List<LanguageSwitchEntryViewModel> Entries { get; set; }

        public LanguageSwitchViewModel()
        {
            Entries = new List<LanguageSwitchEntryViewModel>();
        }
    }
}
