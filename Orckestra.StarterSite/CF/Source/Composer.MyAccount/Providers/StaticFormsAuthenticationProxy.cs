using System;
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

        public void SetAuthCookie(string userName, bool createPersistentCookie, string userData)
        {
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
            HttpContext.Current.Response.Cookies.Add(authCookie);
        }

        public void SetAuthCookie(string userName, int timeoutInMinutes, bool createPersistentCookie, string userData, bool requireSsl)
        {
            var expireDate = DateTime.Now.AddMinutes(timeoutInMinutes);
            var ticket = new FormsAuthenticationTicket(1, userName, DateTime.Now, expireDate, createPersistentCookie, userData);

            var encrypted = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted)
            {
                Expires = expireDate,
                HttpOnly = true,
                Secure = requireSsl
            };

            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}
