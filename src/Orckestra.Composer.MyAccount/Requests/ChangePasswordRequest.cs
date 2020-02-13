using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.MyAccount.Requests
{
    /// <summary>
    /// WebApi Request to Change the password of the currently logged in User
    /// </summary>
    public class ChangePasswordRequest
    {
        /// <summary>
        /// (Mandatory)
        /// The current password for the account to modify
        /// </summary>
        [Required]
        public string OldPassword { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The new password to set.
        /// </summary>
        [Required]
        public string NewPassword { get; set; }

        /// <summary>
        /// (Optional)
        /// ReturnUrl to be used on client side to redirect after changing the password
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}