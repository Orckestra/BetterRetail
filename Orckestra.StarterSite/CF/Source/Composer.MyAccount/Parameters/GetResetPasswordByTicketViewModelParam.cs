using System.Globalization;

namespace Orckestra.Composer.MyAccount.Parameters
{
    public class GetResetPasswordByTicketViewModelParam
    {
        /// <summary>
        /// (Mandatory)
        /// The culture to use for any displayed values.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// (Mandatory)
        /// Encrypted password reset ticket send to the Customer using the
        /// SendResetPasswordInstruction method.
        /// </summary>
        public string Ticket { get; set; }
    }
}
