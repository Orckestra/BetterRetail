using Orckestra.Composer.Enums;
using Orckestra.Composer.ViewModels.Home;

namespace Orckestra.Composer.ViewModels.MenuNavigation
{
    public sealed class OptionalLinkEntryViewModel : BaseViewModel, IMenuEntryViewModel
    {
        public OptionalLinkEntryViewModel()
        {
            MenuType = MenuTypeEnum.Optional;
        }
        public MenuTypeEnum MenuType { get; set; }
        public string DisplayName { get; set; }
        public string Url { get; set; }
        public string UrlTarget { get; set; }
        public string CssClass { get; set;  }
    }
}
