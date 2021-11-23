using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Orckestra.Composer.Enums;

namespace Orckestra.Composer.ViewModels.Home
{
    public sealed class HomeMainMenuEntryViewModel : BaseViewModel, IMenuEntryViewModel
    {
        public HomeMainMenuEntryViewModel()
        {
            Children = Enumerable.Empty<HomeMainMenuEntryViewModel>();
            MenuType = MenuTypeEnum.Principal;
        }

        public string DisplayName { get; set; }

        public string Url { get; set; }     

        public HomeNavigationImageViewModel Image { get; set; }

        public string UrlTarget { get; set; }

        public MenuTypeEnum MenuType { get; set; }

        public string CssClass { get; set; }

        private string _cssClassName;
        public string CssClassName
        {
            get => _cssClassName;
            set => _cssClassName = string.Join(" ", value.Split(','));
        }

        public IEnumerable<IMenuEntryViewModel> Children { get; set; }

        public bool HasChildren
        {
            get { return Children.Any(); }
        }
    }
}