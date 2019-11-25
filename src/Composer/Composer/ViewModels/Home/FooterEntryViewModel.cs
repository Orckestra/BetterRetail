using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Orckestra.Composer.Enums;

namespace Orckestra.Composer.ViewModels.Home
{
    public sealed class FooterEntryViewModel : BaseViewModel, IFooterEntryViewModel
    {
        public FooterEntryViewModel()
        {
            Children = Enumerable.Empty<FooterEntryViewModel>();
            FooterType = FooterTypeEnum.Principal;
        }

        public string DisplayName { get; set; }

        public string Url { get; set; }  
        
        public string UrlTarget { get; set; }

        public string CssClass { get; set; }
        
        public FooterTypeEnum FooterType { get; set; }

        public IEnumerable<IFooterEntryViewModel> Children { get; set; }

        public bool HasChildren => Children.Any();
    }
}