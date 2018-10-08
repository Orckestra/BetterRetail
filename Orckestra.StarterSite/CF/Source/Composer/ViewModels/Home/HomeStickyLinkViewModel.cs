using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Enums;

namespace Orckestra.Composer.ViewModels.Home
{
    public sealed class HomeStickyLinkViewModel : BaseViewModel, IMenuEntryViewModel
    {
        public string DisplayName { get; set; }

        public string Url { get; set; }

        public string UrlTarget { get; set; }

        public MenuTypeEnum MenuType { get; set; }
    }
}
