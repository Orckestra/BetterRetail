using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Services
{
    public interface IFixCartService
    {
        /// <summary>
        /// Fix cart missing informations like empty payments or fulfillment location.
        /// This method will be useless when overture will always return a valid cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<ProcessedCart> FixCartAsync(FixCartParam param);

        Task<ProcessedCart> AddPaymentIfRequired(FixCartParam param);

        Task<ProcessedCart> SetFulfillmentLocationIfRequired(FixCartParam param);

    }
}
