using System.Collections.Generic;
using Orckestra.Composer.Store.Models;
using Orckestra.Composer.Store.Providers;

namespace Orckestra.Composer.Store.Parameters
{
    public class GetMapClustersParam
    {
        public int ZoomLevel { get; set; }
        public  Coordinate SearchPoint { get; set; }
        public IList<IGeoCoordinate> Locations { get; set; }
    }
}
