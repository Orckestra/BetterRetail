using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.MyAccount.Requests
{
    /// <summary>
    /// WebApi Request to Request some Reste password instructions
    /// </summary>
    public class ForgotPasswordRequest
    {
        /// <summary>
        /// (Mandatory)
        /// The unique Email address of the Customer who needs instructions
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
