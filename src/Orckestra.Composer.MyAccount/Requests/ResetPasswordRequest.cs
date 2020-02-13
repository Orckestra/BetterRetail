using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.MyAccount.Requests
{
    /// <summary>
    /// WebApi Request to reset the password for a given Customer
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>
        /// (Mandatory)
        /// Encrypted password reset ticket send to the Customer using the
        /// SendResetPasswordInstruction method.
        /// </summary>
        [Required]
        public string Ticket { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The new password to set
        /// </summary>
        [Required]
        public string NewPassword { get; set; }

        /// <summary>
        /// (Optional)
        /// ReturnUrl to be used on client side to redirect after logout
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
