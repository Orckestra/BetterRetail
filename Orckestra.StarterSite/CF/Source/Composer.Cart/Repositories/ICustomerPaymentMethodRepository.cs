using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Repositories
{
    /// <summary>
    /// Repository for interacting with Customers
    /// Customers represents entities which have the ability to buy products. 
    /// </summary>
    public interface ICustomerPaymentMethodRepository
    {        
        /// <summary>
        /// Get the Payment methods available for a customer.
        /// </summary>
        /// <param name="param">GetPaymentMethodsParam</param>
        /// <returns>A List of PaymentMethod</returns>
        Task<List<PaymentMethod>> GetCustomerPaymentMethodsAsync(GetCustomerPaymentMethodsParam param);
    }
}
