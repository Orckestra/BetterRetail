using System.Globalization;

namespace Orckestra.Composer.Cart.Parameters
{
    public class EstimateShippingParam
    {
        /// <summary>
        /// CultureInfo of the request.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Cart to update. The cart will be changed, but the method will not
        /// save anything.
        /// </summary>
        public Overture.ServiceModel.Orders.Cart Cart { get; set; }

        /// <summary>
        /// Indicates that the method should force or not the use
        /// of the cheapest Shipping method. If false, will only set the shipping
        /// method if it is not already set.
        /// </summary>
        public bool ForceUpdate { get; set; }
    }
}
