using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetShippingMethodsScopeParam
    {

        /// <summary>
        /// The ScopeId where to find the shipping methods
        /// Required
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// The culture for returned Shipping methods info
        /// Optional
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

    }
}
