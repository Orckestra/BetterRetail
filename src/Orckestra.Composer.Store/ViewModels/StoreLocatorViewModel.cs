using System.Collections.Generic;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Store.ViewModels
{
    public sealed class StoreLocatorViewModel : BaseViewModel
    {
        public IList<StoreViewModel> Stores { get; set; }

        public IList<StoreClusterViewModel> Markers { get; set; }

        public StoreGeoCoordinate NearestStoreCoordinate { get; set; }
        public double NearestDistance { get; set; }
        public string LengthMeasureUnit { get; set; }

        public string PostedAddress { get; set; }

        public string StoresDirectoryUrl { get; set; }

        public StorePageViewModel NextPage { get; set; }
    }
}
