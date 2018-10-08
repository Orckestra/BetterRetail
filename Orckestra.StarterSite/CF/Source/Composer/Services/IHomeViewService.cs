
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Mvc;
using Orckestra.Composer.ViewModels.Home;
using Orckestra.Composer.ViewModels.MenuNavigation;
using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Services
{
    public interface IHomeViewService
    {
        /// <summary>
        /// Get the view model for rendering the Home Logo
        /// </summary>
        /// <param name="urlHelper">Mvc UrlHelper bound to the current requestContext</param>
        /// <param name="culture">culture for the logo, urls and alt texts</param>
        /// <returns>ViewModel ready to render</returns>
        Task<HomeLogoViewModel> GetHomeLogoViewModel(UrlHelper urlHelper, CultureInfo culture);

        /// <summary>
        /// Get the view model for rendering the main menu
        /// </summary>
        /// <param name="param">Parameter object used to create the view model</param>        
        /// <returns>Main Menu ViewModel</returns>
        Task<HomeMainMenuViewModel> GetHomeMainMenuViewModel(GetHomeMainMenuParam param);

        /// <summary>
        /// Get the view model for rendering the sticky links
        /// </summary>
        /// <param name="param">Parameter object used to create the view model</param>        
        /// <returns>Sticky Links ViewModel</returns>
        Task<HomeStickyLinksViewModel> GetStickyLinksViewModel(GetStickyLinksParam param);
		
        /// <summary>
        /// Get the view model for rendering optional links
        /// </summary>
        /// <param name="culture">culture for optional links, urls and alt texts</param>
        /// <returns>ViewModel ready to render</returns>
        Task<OptionalLinkViewModel> GetOptionalLinksViewModel(CultureInfo culture);

        /// <summary>
        /// Get the view model for rendering footer optional links
        /// </summary>
        /// <param name="culture">culture for optional links, urls and alt texts</param>
        /// <returns>ViewModel ready to render</returns>
        Task<OptionalLinkViewModel> GetFooterOptionalLinksViewModel(CultureInfo culture);

        /// <summary>
        /// Get the copyright
        /// </summary>
        /// <param name="culture">culture for optional links, urls and alt texts</param>
        /// <returns>string corresponding to the copyright</returns>
        Task<string> GetCopyright(CultureInfo culture);

        /// <summary>
        /// Get the view model for rendering the footer
        /// </summary>
        /// <param name="param">Parameter object used to create the view model</param>        
        /// <returns>Footer ViewModel</returns>
        Task<FooterViewModel> GetFooterViewModel(GetFooterParam param);


    }
}
