using System.Web;
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

        public HttpCookie GetAuthCookie(string userName, bool createPersistentCookie)
        {
            return FormsAuthentication.GetAuthCookie(userName, createPersistentCookie);
        }

        public FormsAuthenticationTicket Decrypt(string encryptedTicket)
        {
            return FormsAuthentication.Decrypt(encryptedTicket);
        }

        public HttpCookie GetAuthCookie(string userName, bool createPersistentCookie, string userData)
        {
            SetAuthCookie(userName, createPersistentCookie);
            HttpCookie authCookie = GetAuthCookie(userName, createPersistentCookie);
            FormsAuthenticationTicket ticket = Decrypt(authCookie.Value);
            FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(
                ticket.Version,
                ticket.Name,
                ticket.IssueDate,
                ticket.Expiration,
                ticket.IsPersistent,
                userData
            );
            authCookie.Value = Encrypt(newTicket);
            return authCookie;
        }
    }
}
