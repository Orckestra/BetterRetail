using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Composite.Data;
using Orckestra.Composer.CompositeC1.DataTypes.Navigation;
using Orckestra.Composer.CompositeC1.Utils;
using Orckestra.Composer.Enums;
using Orckestra.Composer.ViewModels.Home;
using Orckestra.Composer.ViewModels.MenuNavigation;

namespace Orckestra.Composer.CompositeC1.Mappers
{
    public class NavigationMapper : INavigationMapper
    {
        private readonly GoogleAnalyticsNavigationUrlProvider _analyticsNavigationUrlHelper;

        public NavigationMapper(GoogleAnalyticsNavigationUrlProvider analyticsNavigationUrlHelper)
        {
            _analyticsNavigationUrlHelper = analyticsNavigationUrlHelper ?? throw new ArgumentNullException(nameof(analyticsNavigationUrlHelper));
        }

        public virtual IEnumerable<IMenuEntryViewModel> MapMainMenuItems(List<MainMenu> mainMenuItems, Guid? parentId = null)
        {
            return mainMenuItems
                .Where(x=> x.ParentId == parentId && C1Helper.IsUrlPagePublished(x.Url))
                .OrderBy(el => el.Order)
                .Select(li => new HomeMainMenuEntryViewModel()
                {
                    DisplayName = li.DisplayName,
                    Url = _analyticsNavigationUrlHelper.BuildUrl(li, mainMenuItems, GoogleAnalyticsNavigationUrlProvider.MenuOrigin.Dropdown),
                    Image = MapNavigationImage(GetNavigationImage(li.Id, new CultureInfo(li.SourceCultureName))),
                    CssClass = C1Helper.GetCssStyleValue(li.CssStyle),
                    UrlTarget = C1Helper.GetUrlTargetValue(li.Target), 
                    Children = MapMainMenuItems(mainMenuItems, li.Id),
                    MenuType = MenuTypeEnum.Principal
                }).ToList();
        }

        public virtual HomeNavigationImageViewModel MapNavigationImage(NavigationImage navigationImage)
        {
            if (navigationImage == null) return new HomeNavigationImageViewModel();

            return new HomeNavigationImageViewModel()
            {
                ImageLabel = navigationImage.ImageLabel, ImageSource = C1Helper.GetMediaUrl(navigationImage.ImageSource), ImageUrl = navigationImage.ImageUrl, ImageUrlTarget = C1Helper.GetUrlTargetValue(navigationImage.Target)
            };
        }

        public virtual IEnumerable<IMenuEntryViewModel> MapHeaderOptionalLinksItems(List<HeaderOptionalLink> optionalLinksItems)
        {
            return optionalLinksItems.OrderBy(el => el.Order).Select(li => new OptionalLinkEntryViewModel()
            {
                DisplayName = li.DisplayName,
                Url = _analyticsNavigationUrlHelper.BuildUrl(li, GoogleAnalyticsNavigationUrlProvider.MenuOrigin.Dropdown),                
                UrlTarget = C1Helper.GetUrlTargetValue(li.Target),
                CssClass = C1Helper.GetCssStyleValue(li.CssStyle)
            }).ToList();
        }

        public virtual IEnumerable<IMenuEntryViewModel> MapFooterOptionalLinksItems(List<FooterOptionalLink> optionalLinksItems)
        {
            return optionalLinksItems.OrderBy(el => el.Order).Select(li => new OptionalLinkEntryViewModel()
            {
                DisplayName = li.DisplayName,
                Url = _analyticsNavigationUrlHelper.BuildUrl(li, GoogleAnalyticsNavigationUrlProvider.MenuOrigin.Footer),                
                UrlTarget = C1Helper.GetUrlTargetValue(li.Target),
                CssClass = C1Helper.GetCssStyleValue(li.CssStyle)
            }).ToList();
        }

        public virtual IEnumerable<IFooterEntryViewModel> MapFooterItems(List<Footer> footer, Guid? parentId = null)
        {
            return footer.Where(x => x.ParentId == parentId && C1Helper.IsUrlPagePublished(x.Url)).OrderBy(el => el.Order).Select(li => new FooterEntryViewModel()
            {
                DisplayName = li.DisplayName,
                Url = _analyticsNavigationUrlHelper.BuildUrl(li, footer, GoogleAnalyticsNavigationUrlProvider.MenuOrigin.Footer),                
                CssClass = C1Helper.GetCssStyleValue(li.CssStyle),
                UrlTarget = C1Helper.GetUrlTargetValue(li.Target),
                Children = MapFooterItems(footer, li.Id)
            }).ToList();
        }

        public virtual IEnumerable<IMenuEntryViewModel> MapStickyMainMenuItems(List<StickyHeader> mainMenuItems)
        {
            return mainMenuItems.OrderBy(el => el.Order).Select(li => new HomeStickyLinkViewModel()
            {
                DisplayName = li.DisplayName,
                Url = _analyticsNavigationUrlHelper.BuildUrl(li, GoogleAnalyticsNavigationUrlProvider.MenuOrigin.Sticky),                
                UrlTarget = C1Helper.GetUrlTargetValue(li.Target)
            }).ToList();
        }

        public virtual NavigationImage GetNavigationImage(Guid menuItemId, CultureInfo cultureInfo)
        {
            using (DataConnection data = new DataConnection(cultureInfo))
            {
                return data.Get<NavigationImage>().FirstOrDefault(x => x.MenuItemParentId == menuItemId);
            }
        }
    }
}