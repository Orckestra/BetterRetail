using System.Collections.Generic;
using Composite.Data;

namespace Orckestra.Composer.GraphQL.Types
{
    public class DataInput<TSourceType> where TSourceType : class, IData
    {
        public TSourceType Data { get; set; }

        public Dictionary<string, object> PropertyBag { get; set; }
    }
}
