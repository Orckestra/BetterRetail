using System.Web;
using System.Web.Security;

namespace Orckestra.Composer.MyAccount.Providers
{
    /// <summary>
    /// For Unit test purposes
    /// </summary>
    internal interface IFormsAuthenticationProxy
    {
        void SetAuthCookie(string userName, bool createPersistentCookie);
        void SignOut();
        string Encrypt(FormsAuthenticationTicket ticket);
        string FormsCookieName { get; }
        HttpCookie GetAuthCookie(string userName, bool createPersistentCookie);
        FormsAuthenticationTicket Decrypt(string encryptedTicket);
        void SetAuthCookie(string userName, bool createPersistentCookie, string userData);
        void SetAuthCookie(string name, int timeoutInMinutes, bool isPersistent, string userData, bool requireSsl);
    }
}
