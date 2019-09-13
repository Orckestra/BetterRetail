using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Overture.ServiceModel.Customers;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Repositories
{
    public interface IPaymentRepository
    {
        /// <summary>
        /// Gets all payments hold by a cart.
        /// </summary>
        /// <param name="param">Parameters used to make the query.</param>
        /// <returns>A list of Payments.</returns>
        Task<List<Payment>> GetCartPaymentsAsync(GetCartPaymentsParam param);

        /// <summary>
        /// Get the Payment methods available for a cart.
        /// </summary>
        /// <param name="param">GetPaymentMethodsParam</param>
        /// <returns>A List of PaymentMethod</returns>
        Task<List<PaymentMethod>> GetPaymentMethodsAsync(GetPaymentMethodsParam param);

        /// <summary>
        /// Updates the payment method of a given Payment.
        /// </summary>
        /// <param name="param">UpdatePaymentMethodParam</param>
        /// <returns>The updated processed cart.</returns>
        Task<ProcessedCart> UpdatePaymentMethodAsync(UpdatePaymentMethodParam param);

        /// <summary>
        /// Initializes the specified payment in the cart.
        /// </summary>
        /// <param name="param">Parameters used to initialized the Payment.</param>
        /// <returns>The updated cart.</returns>
        Task<Overture.ServiceModel.Orders.Cart> InitializePaymentAsync(InitializePaymentParam param);

        /// <summary>
        /// Voids a payment and generates a new one.
        /// </summary>
        /// <param name="param">Parameters used to void the payment.</param>
        /// <returns>The updated cart.</returns>
        Task<Overture.ServiceModel.Orders.Cart> VoidPaymentAsync(VoidOrRemovePaymentParam param);

        /// <summary>
        /// Removes a payment from a cart.
        /// </summary>
        /// <param name="param">Parameters used to remove the payment.</param>
        /// <returns>The updated cart.</returns>
        Task<ProcessedCart> RemovePaymentAsync(VoidOrRemovePaymentParam param);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task RemovePaymentMethodAsync(RemovePaymentMethodParam param);

        /// <summary>
        /// Get the customer payment profile for the specified scope
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<List<PaymentProfile>> GetCustomerPaymentProfiles(GetCustomerPaymentProfilesParam param);


        /// <summary>
        /// Get a list of payment method for a cystomer and provider 
        /// </summary>
        /// <returns></returns>
        Task<List<PaymentMethod>> GetCustomerPaymentMethodForProviderAsync(GetCustomerPaymentMethodsForProviderParam param);

        /// <summary>
        /// Get a specific payment by Id
        /// </summary>
        /// <returns></returns>
        Task<Payment> GetPaymentAsync(GetPaymentParam param);
    }
}
