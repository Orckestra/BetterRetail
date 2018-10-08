using System;
using System.Globalization;

namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Builder params for creating the <see cref="Orckestra.Composer.MyAccount.ViewModels.AccountHeaderViewModel"/>
    /// </summary>
    public class GetAccountHeaderViewModelParam
    {
        /// <summary>
        /// (Mandatory)
        /// The culture to use for any displayed values.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// The authenticated user id(if any)
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The scope to get the user
        /// </summary>
        public string Scope { get; set; }
    }
}
