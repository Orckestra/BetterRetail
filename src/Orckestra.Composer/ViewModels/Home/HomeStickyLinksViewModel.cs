using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.ViewModels.Home
{
    public sealed class HomeStickyLinksViewModel : BaseViewModel
    {
        public HomeStickyLinksViewModel()
        {
            Entries = Enumerable.Empty<HomeStickyLinkViewModel>();
        }

        public IEnumerable<IMenuEntryViewModel> Entries { get; set; }
    }
}
