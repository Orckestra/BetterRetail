using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Parameters
{
    public class GetRecurringOrderTemplatesParam
    {
        public CultureInfo CultureInfo { get; set; }
        public string Scope { get; set; }
        public string BaseUrl { get; set; }
        public Guid CustomerId { get; set; }
    }
}
