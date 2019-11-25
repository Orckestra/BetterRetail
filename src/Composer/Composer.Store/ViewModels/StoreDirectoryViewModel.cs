using System.Collections.Generic;
using Orckestra.Composer.Store.Extentions;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Store.ViewModels
{
    public sealed class StoreDirectoryViewModel : BaseViewModel
    {
        public string StoreLocatorPageUrl { get; set; }
        public IList<StoreDirectoryGroupViewModel> Groups { get; set; }
        public StorePaginationViewModel Pagination { get; set; }
    }
}
