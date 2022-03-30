using System;
using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.CompositeC1.DataTypes.Navigation;
using Orckestra.Composer.CompositeC1.Providers.MainMenu;
using Orckestra.Composer.ViewModels.Home;

namespace Orckestra.Composer.CompositeC1.Mappers
{
    public interface INavigationMapper
    {
        IEnumerable<IMenuEntryViewModel> MapMainMenuItems(List<MainMenuItemWrapper> mainMenuItems, Guid? parentId = null);
        HomeNavigationImageViewModel MapNavigationImage(NavigationImage navigationImage);
        IEnumerable<IMenuEntryViewModel> MapHeaderOptionalLinksItems(List<HeaderOptionalLink> optionalLinksItems);
        IEnumerable<IMenuEntryViewModel> MapFooterOptionalLinksItems(List<FooterOptionalLink> optionalLinksItems);
        IEnumerable<IFooterEntryViewModel> MapFooterItems(List<Footer> footer, Guid? parentId = null);
        IEnumerable<IMenuEntryViewModel> MapStickyMainMenuItems(List<StickyHeader> mainMenuItems);
        NavigationImage GetNavigationImage(Guid menuItemId, CultureInfo cultureInfo);
    }
}