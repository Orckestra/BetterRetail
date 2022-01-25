using System.Globalization;

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
