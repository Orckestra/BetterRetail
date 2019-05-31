using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.ViewModels;

namespace Orckestra.Composer.Cart.Services
{
    /// <summary>
    /// Service for building ViewModel relative to Customers
    /// Customers are users with the ability to buy products
    /// </summary>
    public interface ICustomerPaymentMethodViewService
    {
        Task<CustomerPaymentViewModel> GetCustomerPaymentMethodsAsync(GetCustomerPaymentMethodsParam param);
    }
}
