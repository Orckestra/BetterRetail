namespace Orckestra.Composer.MyAccount.Parameters
{
    /// <summary>
    /// Service call param to retreive a single Customer based on a password reset ticket
    /// </summary>
    public class GetCustomerByTicketParam
    {
        /// <summary>
        /// (Mandatory)
        /// Encrypted password reset ticket send to the Customer using the
        /// SendResetPasswordInstruction method.
        /// </summary>
        public string Ticket { get; set; }
    }
}
