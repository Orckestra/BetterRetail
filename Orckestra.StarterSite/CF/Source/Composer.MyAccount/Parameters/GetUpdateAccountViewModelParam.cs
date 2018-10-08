using System;
using System.Globalization;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Builder params for creating the <see cref="Orckestra.Composer.MyAccount.ViewModels.UpdateAccountViewModel"/>
    /// </summary>
    public class GetUpdateAccountViewModelParam
    {
        /// <summary>
        /// (Mandatory)
        /// The culture to use for any displayed values
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The displayed/updated Customer id
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The scope to get the user
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// (Optional) 
        /// The update results
        /// </summary>
        public MyAccountStatus? Status { get; set; }

        /// <summary>
        /// (Optional) 
        /// ReturnUrl to be used on client side to redirect on success
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
