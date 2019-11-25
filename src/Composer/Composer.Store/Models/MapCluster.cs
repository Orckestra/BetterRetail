using Orckestra.Composer.Store.Providers;

namespace Orckestra.Composer.Store.Models
{
    using System.Collections.Generic;

    public class MapCluster
    {
        public MapCluster()
        {
            Locations = new List<IGeoCoordinate>();
            ItemsCount = 0;
        }

        public int ItemsCount { get; set; }
        public Bounds Tile { get; set; }
        public Coordinate Center { get; set; }

        public List<IGeoCoordinate> Locations { get; private set; }

        public string StoreNumber { get; set; }

        public int SearchIndex { get; set; }

    }
}