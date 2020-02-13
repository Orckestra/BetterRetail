using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class GetPaymentProvidersParam
    {
        /// <summary>
        /// Scope of providers.
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Culture in which to make the query.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }
    }
}
