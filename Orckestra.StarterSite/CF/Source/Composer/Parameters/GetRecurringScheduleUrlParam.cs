using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Parameters
{
     /// <summary>
     /// Parameter for <see cref="IRecurringScheduleUrlProvider" />
     /// </summary>
    public class GetRecurringScheduleUrlParam
    {
        /// <summary>
        /// The culture info in which language the data will be returned
        /// Optional
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// ReturnUrl to preserve
        /// Optional
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
