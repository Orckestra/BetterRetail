namespace Orckestra.Composer.ViewModels.LanguageSwitch
{
    public sealed class LanguageSwitchEntryViewModel : BaseViewModel
    {
        public string Url { get; set; }

        public string ShortDisplayName { get; set; }
        public string DisplayName { get; set; }
        public string CultureName { get; set; }

        public bool IsCurrent { get; set; }
    }
}