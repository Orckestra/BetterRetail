using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Parameters
{
    public class FixCartParam : BaseCartParam
    {
        /// <summary>
        /// The cart to fix
        /// </summary>
        public ProcessedCart Cart { get; set; }
    }
}
