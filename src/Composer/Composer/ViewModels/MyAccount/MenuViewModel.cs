using System.Collections.Generic;

namespace Orckestra.Composer.ViewModels.MyAccount
{
    public sealed class MenuViewModel : BaseViewModel
    {
        public List<MenuItemViewModel> MenuItems { get; set; }

        public MenuViewModel()
        {
            MenuItems = new List<MenuItemViewModel>();
        }
    }
}