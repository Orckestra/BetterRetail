using System;
using System.Collections.Generic;

namespace Orckestra.Composer.CompositeC1.Providers.MainMenu
{
    public interface IMainMenuItemsProvider
    {
        IEnumerable<MainMenuItemWrapper> GetMainMenuItems(Guid websiteId);
        bool IsActive(Guid websiteId);
    }
}
