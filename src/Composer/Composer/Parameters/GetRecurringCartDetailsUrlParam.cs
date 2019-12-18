using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Parameters
{
    public class GetRecurringCartDetailsUrlParam
    {
        public CultureInfo CultureInfo { get; set; }
        public string ReturnUrl { get; set; }
        public string RecurringCartName { get; set; }
    }
}
