using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.ViewModels.Home
{
    public sealed class FooterViewModel : BaseViewModel
    {
        public IEnumerable<IFooterEntryViewModel> Entries { get; set; }
    }
}
