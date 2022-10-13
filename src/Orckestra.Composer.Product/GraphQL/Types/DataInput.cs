using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Composite.Data;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.Product.GraphQL.Types
{
    public class DataInput<TSourceType> where TSourceType : class, IData
    {
        public TSourceType Data { get; set; }

        public Dictionary<string, object> PropertyBag { get; set; }
    }
}
