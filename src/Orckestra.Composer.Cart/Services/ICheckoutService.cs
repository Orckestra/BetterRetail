using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Services
{
    /// <summary>
    /// Service for dealing with Checkout.
    /// </summary>
    public interface ICheckoutService
    {
        /// <summary>
        /// Update a Cart during the checkout process.
        /// </summary>
        /// <param name="param">UpdateCheckoutCartParam</param>
        Task<UpdateCartResultViewModel> UpdateCheckoutCartAsync(UpdateCheckoutCartParam param);

        /// <summary>
        /// Complete the checkout process
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The Order</returns>
        Task<CompleteCheckoutViewModel> CompleteCheckoutAsync(CompleteCheckoutParam param);

        Task<Orckestra.Overture.ServiceModel.Orders.Cart> TokenizePaymentAsync(TokenizePaymentParam param);
    }
}
