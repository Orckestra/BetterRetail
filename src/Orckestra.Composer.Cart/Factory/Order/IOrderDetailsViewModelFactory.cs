using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.ViewModels.Order;

namespace Orckestra.Composer.Cart.Factory.Order
{
    public interface IOrderDetailsViewModelFactory
    {
        /// <summary>
        /// Creates an OrderDetailViewModel, containing all the informations about the order, the shipments and lineitems.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        OrderDetailViewModel CreateViewModel(CreateOrderDetailViewModelParam param);

        /// <summary>
        /// Creates a LightOrderDetailViewModel
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        LightOrderDetailViewModel CreateLightViewModel(CreateOrderDetailViewModelParam param);
    }
}
