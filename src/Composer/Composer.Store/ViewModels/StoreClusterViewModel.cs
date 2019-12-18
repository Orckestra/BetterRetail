using Orckestra.Composer.Store.Models;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Store.ViewModels
{
    public sealed class StoreClusterViewModel : BaseViewModel
    {

        public int ItemsCount { get; set; }
        public Bounds Tile { get; set; }
        public Coordinate Center { get; set; }

        public string StoreNumber { get; set; }

        public int SearchIndex { get; set; }
    }
}
