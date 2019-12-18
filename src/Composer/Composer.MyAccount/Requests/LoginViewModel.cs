using System.ComponentModel.DataAnnotations;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.MyAccount.Requests
{
    /// <summary>
    /// WebApi Request to Authenticate a user
    /// </summary>
    public sealed class LoginViewModel : BaseViewModel
    {
        /// <summary>
        /// (Mandatory)
        /// The unique login Name of the User to authenticate
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Username  { get; set; }

        /// <summary>
        /// (Mandatory)
        /// The login Password of the User to authenticate
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Password  { get; set; }

        /// <summary>
        /// (Optional) 
        /// ReturnUrl to be used on client side to redirect on success
        /// </summary>
        public string ReturnUrl { get; set; }

        public bool IsRememberMe { get; set; }
    }
}
