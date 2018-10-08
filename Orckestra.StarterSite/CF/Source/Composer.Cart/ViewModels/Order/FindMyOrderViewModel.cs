using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.ViewModels.Order
{
    public sealed class FindMyOrderViewModel : BaseViewModel
    {
        /// <summary>
        /// The email used to retrieve the order.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The order number used to retrieve the order.
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Indicates if the order was found.
        /// </summary>
        public bool OrderNotFound { get; set; }
    }
}
