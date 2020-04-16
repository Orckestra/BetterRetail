using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.ViewModels.Home;

namespace Orckestra.Composer.ViewModels.MenuNavigation
{
    public sealed class OptionalLinkViewModel : BaseViewModel
    {
        public OptionalLinkViewModel()
        {
            OptionalLinkEntries = new List<OptionalLinkEntryViewModel>();
        }

        public IEnumerable<IMenuEntryViewModel> OptionalLinkEntries { get; set; }
    }
}
