using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Composer.Cart.ViewModels.Order;

namespace Orckestra.Composer.Cart.Factory.Order
{
    public interface IOrderHistoryViewModelFactory
    {
        /// <summary>
        /// Creates the view model.
        /// </summary>
        /// <param name="param">The parameters for creating the loading view model.</param>
        /// <returns></returns>
        OrderHistoryViewModel CreateViewModel(GetOrderHistoryViewModelParam param);
    }
}