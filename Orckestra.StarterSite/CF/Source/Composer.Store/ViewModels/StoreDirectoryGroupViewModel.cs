using System.Collections.Generic;
using Orckestra.Composer.Store.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Store.Extentions
{
    public sealed class StoreDirectoryGroupViewModel : BaseViewModel
    {
        public object Key { get; set; }
        public string DisplayName { get; set; }
        public int Count { get; set; }
        public IList<StoreViewModel> Items { get; set; }
        public IList<StoreDirectoryAnchorViewModel> Anchors { get; set; }
        public IList<StoreDirectoryGroupViewModel> SubGroups { get; set; }
    }
}
