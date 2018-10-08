using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.MyAccount.ViewModels
{
    public sealed class PreferredLanguageViewModel : BaseViewModel
    {
        /// <summary>
        /// The Culture ISO code
        /// </summary>
        public string IsoCode { get; set; }

        /// <summary>
        /// The culture Display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Indicates if it is the customer preferred language.
        /// </summary>
        public bool IsSelected { get; set; }
    }

}
