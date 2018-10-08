using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Search.Parameters
{
    public class CategorySearchCriteria : SearchCriteria
    {
        public string CategoryId { get; set; }
        public bool CategoryHasFacets { get; set; }
    }
}
