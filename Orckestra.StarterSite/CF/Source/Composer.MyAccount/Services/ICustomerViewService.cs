using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.MyAccount.Services
{
    /// <summary>
    /// Service for building ViewModel relative to Customers
    /// Customers are users with the ability to buy products
    /// </summary>
    public interface ICustomerViewService
    {
        /// <summary>
        /// Gets the <see cref="AccountHeaderViewModel"/> to greet a given Customer.
        /// </summary>
        /// <param name="param">Builder params <see cref="GetAccountHeaderViewModelParam"/></param>
        /// <returns></returns>
        Task<AccountHeaderViewModel> GetAccountHeaderViewModelAsync(GetAccountHeaderViewModelParam param);
        /// <summary>
        /// Gets the <see cref="AccountStatusViewModel"/> to greet a given Customer.
        /// </summary>
        /// <param name="param">Builder params <see cref="GetAccountStatusViewModelParam"/></param>
        /// <returns></returns>
        Task<AccountStatusViewModel> GetAccountStatusViewModelAsync(GetAccountStatusViewModelParam param);

        /// <summary>
        /// Gets the <see cref="UpdateAccountViewModel"/> to display the MyInformation Form or Form result
        /// </summary>
        /// <param name="param">Builder params <see cref="GetUpdateAccountViewModelParam"/></param>
        /// <returns></returns>
        Task<UpdateAccountViewModel> GetUpdateAccountViewModelAsync(GetUpdateAccountViewModelParam param);

        /// <summary>
        /// Update the account informations.
        /// </summary>
        /// <param name="param">Builder params <see cref="UpdateAccountParam"/></param>
        /// <returns></returns>
        Task<UpdateAccountViewModel> UpdateAccountAsync(UpdateAccountParam param);
    }
}
