using System.Collections.Generic;
using Orckestra.Composer.Store.Models;
using Orckestra.Composer.Store.Parameters;

namespace Orckestra.Composer.Store.Providers
{
    public interface IMapClustererProvider
    {
        IList<MapCluster> GetMapClusters(GetMapClustersParam param);
    }
}
