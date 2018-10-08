using System.Collections.Generic;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Store.ViewModels
{
    public sealed class StoreInventoryViewModel : BaseViewModel
    {
        public IList<StoreViewModel> Stores { get; set; }
        public StorePageViewModel NextPage { get; set; }
    }
}
