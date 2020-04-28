using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Cart.Services
{
    /// <summary>
    /// Service for dealing with Shipping Methods.
    /// </summary>
    public interface IShippingMethodViewService
    {
        /// <summary>
        /// Get the Shipping methods available for a shipment.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The ShippingMethodsViewModel</returns>
        Task<ShippingMethodsViewModel> GetShippingMethodsAsync(GetShippingMethodsParam param);

        /// <summary>
        /// Get the Shipping methods available for a shipment grouped by type.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The ShippingMethodTypesViewModel</returns>
        Task<ShippingMethodTypesViewModel> GetShippingMethodTypesAsync(GetShippingMethodsParam param);

        bool FilterShippingMethodView(ShippingMethodViewModel sippingMethod);

        int OrderShippingMethodTypeView(ShippingMethodTypeViewModel sippingMethodType);

        /// <summary>
        /// Get the Shipping methods available for a shipment. Calls the GetCart to get the shipment Id.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<ShippingMethodsViewModel> GetRecurringCartShippingMethodsAsync(GetShippingMethodsParam param);
            
        /// <summary>
        /// Set the cheapest shipping method in the cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<CartViewModel> SetCheapestShippingMethodAsync(SetCheapestShippingMethodParam param);

        /// <summary>
        /// Estimates the shipping method. Does not save the cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<ShippingMethodViewModel> EstimateShippingAsync(EstimateShippingParam param);

        /// <summary>
        /// Get the Shipping methods available in the scope.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The ShippingMethodsViewModel</returns>
        Task<RecurringOrdersTemplatesShippingMethodsViewModel> GetShippingMethodsScopeAsync(GetShippingMethodsScopeParam param);

        /// <summary>
        /// Update a recurring cart to a specific shipping method
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<CartViewModel> UpdateRecurringOrderCartShippingMethodAsync(UpdateRecurringOrderCartShippingMethodParam param);
    }
}
