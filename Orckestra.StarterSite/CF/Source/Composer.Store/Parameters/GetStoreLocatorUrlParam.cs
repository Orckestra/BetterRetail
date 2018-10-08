using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Store.Parameters
{
    public class GetStoreLocatorUrlParam
    {
        public string BaseUrl { get; set; }

        public int Page { get; set; }

        public CultureInfo CultureInfo { get; set; }
    }
}
