using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels.Home;
using Orckestra.Composer.ViewModels.MenuNavigation;
using System.Collections.Generic;
using Composite.Core.Routing;
using System.Collections.Specialized;
using Composite.Data;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.DataTypes;
using Orckestra.Composer.CompositeC1.DataTypes.Navigation;
using Orckestra.Composer.CompositeC1.Mappers;
using Orckestra.Composer.CompositeC1.Utils;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Enums;


namespace Orckestra.Composer.CompositeC1.Services
{
    public class HomeViewService : IHomeViewService
    {
        protected INavigationMapper HomeNavigationMapper { get; }
        protected IPageService PageService { get; }
        protected ILocalizationProvider LocalizationProvider { get; }

        public HomeViewService(IPageService pageService, ILocalizationProvider localizationProvider, INavigationMapper homeNavigationMapper)
        {
            if (pageService == null) { throw new ArgumentNullException(nameof(pageService)); }

            PageService = pageService;
            LocalizationProvider = localizationProvider;
            HomeNavigationMapper = homeNavigationMapper;
        }

        public virtual Task<HomeLogoViewModel> GetHomeLogoViewModel(UrlHelper urlHelper, CultureInfo culture)
        {
            var homePage = PageService.GetPage(PagesConfiguration.HomePageId);
            var homePageUrl = PageUrls.BuildUrl(homePage);

            var homeViewModel = new HomeLogoViewModel
            {
                HomeTitle = homePage.MenuTitle,
                HomeLogoUrl = "/UI.Package/Images/better-retail.png",  //TODO read Logo from CMS when available
                HomeUrl = homePageUrl
            };

            return Task.FromResult(homeViewModel);
        }

        public virtual Task<HomeMainMenuViewModel> GetHomeMainMenuViewModel(GetHomeMainMenuParam param)
        {
            using (DataConnection data = new DataConnection(param.CultureInfo))
            {
                var mainMenu = data.Get<MainMenu>().ToList();
                var optionalLinks = data.Get<HeaderOptionalLink>().ToList();

                var mainMenuViewModel = HomeNavigationMapper.MapMainMenuItems(mainMenu);
                var optionalLinksViewModel = HomeNavigationMapper.MapHeaderOptionalLinksItems(optionalLinks);

                var viewModel = new HomeMainMenuViewModel
                {
                    Entries = mainMenuViewModel.Concat(optionalLinksViewModel)
                };

                return Task.FromResult(viewModel);
            }
        }

        public virtual Task<OptionalLinkViewModel> GetOptionalLinksViewModel(CultureInfo culture)
        {
            using (DataConnection data = new DataConnection(CultureInfo.CurrentCulture))
            {
                var optionalLinks = data.Get<HeaderOptionalLink>().ToList();
                var viewModel = new OptionalLinkViewModel
                {
                    OptionalLinkEntries = HomeNavigationMapper.MapHeaderOptionalLinksItems(optionalLinks),
                };
                return Task.FromResult(viewModel);
            }
        }

        public virtual Task<OptionalLinkViewModel> GetFooterOptionalLinksViewModel(CultureInfo culture)
        {
            using (DataConnection data = new DataConnection(culture))
            {
                var optionalLinks = data.Get<FooterOptionalLink>().ToList();
                var viewModel = new OptionalLinkViewModel
                {
                    OptionalLinkEntries = HomeNavigationMapper.MapFooterOptionalLinksItems(optionalLinks),
                };
                return Task.FromResult(viewModel);
            }
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

        public virtual Task<FooterViewModel> GetFooterViewModel(GetFooterParam param)
        {
            using (DataConnection data = new DataConnection(param.CultureInfo))
            {
                var footer = data.Get<Footer>().ToList();
                var viewModel = new FooterViewModel()
                {
                    Entries = HomeNavigationMapper.MapFooterItems(footer),
                };
                return Task.FromResult(viewModel);
            }
        }

        public virtual Task<HomeStickyLinksViewModel> GetStickyLinksViewModel(GetStickyLinksParam param)
        {
            using (DataConnection data = new DataConnection(param.CultureInfo))
            {
                var stickyMainMenu = data.Get<StickyHeader>().ToList();
                var viewModel = new HomeStickyLinksViewModel()
                {
                    Entries = HomeNavigationMapper.MapStickyMainMenuItems(stickyMainMenu)
                };

                return Task.FromResult(viewModel);
            }
        }


     
      
    }
}
