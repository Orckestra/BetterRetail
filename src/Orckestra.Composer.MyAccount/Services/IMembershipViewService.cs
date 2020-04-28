using System.Threading.Tasks;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.ViewModels;

namespace Orckestra.Composer.MyAccount.Services
{
    /// <summary>
    /// Service for building ViewModel relative to Users
    /// </summary>
    public interface IMembershipViewService
    {
        /// <summary>
        /// Get the view model to display the create account form
        /// </summary>
        /// <param name="param">Builder params <see cref="GetCreateAccountViewModelParam"/></param>
        /// <returns>
        /// The view model to display the create account form
        /// </returns>
        CreateAccountViewModel GetCreateAccountViewModel(GetCreateAccountViewModelParam param);

        /// <summary>
        /// Get the view Model to display the Sign In Header
        /// </summary>
        /// <returns>
        /// The view model to display the Sign In Header
        /// </returns>
        Task<SignInHeaderViewModel> GetSignInHeaderModel(GetSignInHeaderParam param);

        /// <summary>
        /// Get the view Model to display a Reset Password Form and Form result
        /// </summary>
        /// <param name="param">Builder params <see cref="GetResetPasswordByTicketViewModelParam"/></param>
        /// <returns>
        /// The view model to display the Reset Password Form
        /// </returns>
        Task<ResetPasswordViewModel> GetCustomerByTicketResetPasswordViewModelAsync(GetResetPasswordByTicketViewModelParam param);

        /// <summary>
        /// Get the view Model to display a Change Password Form and Form result
        /// </summary>
        /// <param name="param">Builder params <see cref="GetCustomerChangePasswordViewModelParam"/></param>
        /// <returns>
        /// The view model to display the Change Password Form
        /// </returns>
        Task<ChangePasswordViewModel> GetChangePasswordViewModelAsync(GetCustomerChangePasswordViewModelParam param);

        /// <summary>
        /// Logs in a customer.
        /// </summary>
        /// <param name="loginParam">Service call params <see cref="LoginParam"/></param>
        /// <returns>
        /// The view model to display the Login Form
        /// </returns>
        Task<LoginViewModel> LoginAsync(LoginParam loginParam);

        /// <summary>
        /// Create a customer account.
        /// </summary>
        /// <param name="createUserParam">Service call params <see cref="CreateUserParam"/></param>
        /// <returns>
        /// The view model to display the Create Account Form
        /// </returns>
        Task<CreateAccountViewModel> RegisterAsync(CreateUserParam createUserParam);

        /// <summary>
        /// Send instructions to the customer on how to reset it's password.
        /// </summary>
        /// <param name="forgotPasswordParam">Service call params <see cref="ForgotPasswordParam"/></param>
        /// <returns>
        /// The Customer who received the instructions and a status representing a possible cause of errors
        /// </returns>
        Task<ForgotPasswordViewModel> ForgotPasswordAsync(ForgotPasswordParam forgotPasswordParam);

        /// <summary>
        /// Sets the new password for the user idenfied by the secure Ticket.
        /// </summary>
        /// <param name="resetPasswordParam">Service call params <see cref="ResetPasswordParam"/></param>
        /// <returns>
        /// The updated Customer and a status representing a possible cause of errors
        /// </returns>
        Task<ResetPasswordViewModel> ResetPasswordAsync(ResetPasswordParam resetPasswordParam);

        /// <summary>
        /// Sets the new password for a customer.
        /// </summary>
        /// <param name="changePasswordParam">Service call params <see cref="ChangePasswordParam"/></param>
        /// <returns>
        ///  The updated Customer and a status representing a possible cause of errors
        /// </returns>
        Task<ChangePasswordViewModel> ChangePasswordAsync(ChangePasswordParam changePasswordParam);

        /// <summary>
        /// Gets the LoginViewModel for a specified customer.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        LoginViewModel GetLoginViewModel(GetLoginViewModelParam param);

        /// <summary>
        /// Return true if the user exist
        /// </summary>
        /// <param name="param">Builder params <see cref="GetCustomerByEmailParam"/></param>
        /// <returns>
        /// The view model is user exist
        /// </returns>
        Task<IsUserExistViewModel> GetIsUserExistViewModelAsync(GetCustomerByEmailParam getCustomerByEmailParam);
    }
}
