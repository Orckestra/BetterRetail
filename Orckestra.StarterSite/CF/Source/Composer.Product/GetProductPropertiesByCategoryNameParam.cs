using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Product
{
    public class GetProductPropertiesByCategoryNameParam
    {
        public string CategoryId { get; set; }
        public string Scope { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public List<string> PropertiesToReturn { get; set; }
    }
}
