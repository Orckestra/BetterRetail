namespace Orckestra.Composer.MyAccount.Responses
{
    /// <summary>
    /// Response sent after user attempts to logout
    /// </summary>
    public class LogoutResponse
    {
        /// <summary>
        /// ReturnUrl to be used on client side to redirect after logout
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
