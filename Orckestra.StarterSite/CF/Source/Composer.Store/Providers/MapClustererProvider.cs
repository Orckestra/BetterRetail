using System.Collections.Generic;
using System.Linq;
using Orckestra.Composer.Store.Models;
using Orckestra.Composer.Store.Parameters;

namespace Orckestra.Composer.Store.Providers
{

    public class MapClustererProvider : IMapClustererProvider
    {
        #region zoom scales

        private static readonly Dictionary<int, double> ZoomLngScales = new Dictionary<int, double>(21)
        {
            {0, 35},
            {1, 20},
            {2, 10},
            {3, 8},
            {4, 4.95},
            {5, 2.5},
            {6, 1.15},
            {7, 0.63},
            {8, 0.3},
            {9, 0.18},
            {10, 0.1},
            {11, 0.05},
            {12, 0.018},
            {13, 0.008},
            {14, 0.004},
            {15, 0.0020},
            {16, 0.0010},
            {17, 0.0004},
            {18, 0.000127},
            {19, 0.000087},
            {20, 0.00003084},
            {21, 0.00001542}
        };

        private static readonly Dictionary<int, double> ZoomLatScales = new Dictionary<int, double>(21)
        {
            {0, 35},
            {1, 20},
            {2, 10},
            {3, 8},
            {4, 3.1},
            {5, 1.75},
            {6, 1},
            {7, 0.5},
            {8, 0.3},
            {9, 0.16},
            {10, 0.1},
            {11, 0.05},
            {12, 0.018},
            {13, 0.008},
            {14, 0.004},
            {15, 0.0018},
            {16, 0.0010},
            {17, 0.0004},
            {18, 0.000127},
            {19, 0.000071},
            {20, 0.00003477},
            {21, 0.00001503}
        };

        #endregion

        public const int XDegreShift = 180;
        public const int YDegreShift = 90;
        private int ZoomLevel { get; set; }

        public virtual IList<MapCluster> GetMapClusters(GetMapClustersParam param)
        {
            ZoomLevel = param.ZoomLevel;
  //          var storesInBounds = param.Locations.Where(store => InBounds(param.MapBounds, store));
            var context = new ClusteringContext();
            foreach (var store in param.Locations)
            {
                AddToClosestCluster(context, store);
            }

            return context.Clusters;
        }

        private void AddToClosestCluster(ClusteringContext context, IGeoCoordinate store)
        {

            var clusterToAdd = context.Clusters.FirstOrDefault(cluster => InBounds(cluster.Tile, store));

            if (clusterToAdd != null)
            {
                AddStoreMarkerToCluster(clusterToAdd, store);
            }
            else
            {
                clusterToAdd = new MapCluster { SearchIndex = context.Clusters.Count + 1 };
                AddStoreMarkerToCluster(clusterToAdd, store);
                context.Clusters.Add(clusterToAdd);
            }
        }

        private void AddStoreMarkerToCluster(MapCluster cluster, IGeoCoordinate store)
        {
            var storeCoordinate = store.GetCoordinate();
            if (cluster.Center == null)
            {
                cluster.Center = storeCoordinate;
            }
            else
            {
                var l = cluster.ItemsCount + 1;
                var lat = (cluster.Center.Lat*(l - 1) + storeCoordinate.Lat)/l;
                var lng = (cluster.Center.Lng*(l - 1) + storeCoordinate.Lng)/l;
                cluster.Center = new Coordinate(lat, lng);
            }

            var xScale = ZoomLngScales[ZoomLevel];
            var yScale = ZoomLatScales[ZoomLevel];

            cluster.Tile = new Bounds(new Coordinate(cluster.Center.Lat - yScale/2, cluster.Center.Lng - xScale/2),
                new Coordinate(cluster.Center.Lat + yScale/2, cluster.Center.Lng + xScale/2));


            cluster.ItemsCount++;
            cluster.StoreNumber = cluster.ItemsCount == 1 ? store.Id : cluster.StoreNumber + ";" + store.Id;
        }

        public bool InBounds(Bounds source, IGeoCoordinate bound)
        {
            var coordinate = bound.GetCoordinate();
            if (coordinate != null)
            {
                return source.Contains(coordinate);
            }
            return false;
        }

        private class ClusteringContext
        {
            public ClusteringContext()
            {
                Clusters = new List<MapCluster>();
            }
            public IList<MapCluster> Clusters { get; private set; }
        }
    }


}