namespace Orckestra.Composer.ViewModels.Home
{
    public sealed class HomeLogoViewModel : BaseViewModel
    {
        /// <summary>
        /// Url for the src to the Home logo
        /// </summary>
        public string HomeLogoUrl { get; set; }

        /// <summary>
        /// Url for the href to the Home Page
        /// </summary>
        public string HomeUrl { get; set; }

        /// <summary>
        /// Localized text for the home alt
        /// </summary>
        public string HomeTitle { get; set; }
    }
}
