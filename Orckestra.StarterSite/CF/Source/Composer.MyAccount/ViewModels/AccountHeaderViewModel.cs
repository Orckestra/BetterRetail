using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.MyAccount.ViewModels
{
    /// <summary>
    /// ViewModel to display the Account Header
    /// </summary>
    public sealed class AccountHeaderViewModel : BaseViewModel
    {
        /// <summary>
        ///  The FirstName for the User account
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///  The LastName for the User account
        /// </summary>
        public string LastName { get; set; }
    }
}
