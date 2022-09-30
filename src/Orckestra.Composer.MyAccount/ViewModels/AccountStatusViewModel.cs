using Orckestra.Composer.Enums;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.MyAccount.ViewModels
{
    /// <summary>
    /// ViewModel to display the Account Header
    /// </summary>
    public sealed class AccountStatusViewModel : BaseViewModel
    {
        /// <summary>
        ///   ['Active' or 'Inactive' or 'RequiresApproval']: The current status of the customer account.
        /// </summary>
        public AccountStatusEnum Status { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
