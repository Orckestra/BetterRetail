using System.Web.Security;

namespace Orckestra.Composer.MyAccount.Providers
{
    /// <summary>
    /// For unit test purposes
    /// </summary>
    internal sealed class StaticFormsAuthenticationProxy : IFormsAuthenticationProxy
    {
        public void SetAuthCookie(string userName, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        public string Encrypt(FormsAuthenticationTicket ticket)
        {
            return FormsAuthentication.Encrypt(ticket);
        }

        public string FormsCookieName
        {
            get { return FormsAuthentication.FormsCookieName; }
        }
    }
}
