
namespace Orckestra.Composer.MyAccount.Requests
{
    /// <summary>
    /// WebApi Request to Logout the currently authenticated user.
    /// </summary>
    public class LogoutRequest
    {
        /// <summary>
        /// (Optional)
        /// Should we preserve the CustomerInfo so the personalization remains available
        /// for the customer even if his action and roles becomes restricted
        /// (IsGuest remains false)
        /// (IsAuthenticated becomes false)
        /// </summary>
        public bool PreserveCustomerInfo { get; set; }

        /// <summary>
        /// (Optional)
        /// ReturnUrl to be used on client side to redirect after logout
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
