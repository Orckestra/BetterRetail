using System.Collections.Generic;

namespace Orckestra.Composer.SearchQuery.Parameters
{
    public class GetInventoryLocationStatuseParam
    {
        public List<string> Skus { get; set; }
        public List<string> InventoryLocationIds { get; set; }
        public string ScopeId { get; internal set; }
    }
}
