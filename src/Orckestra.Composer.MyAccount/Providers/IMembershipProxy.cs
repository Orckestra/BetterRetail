using System.Web.Security;

namespace Orckestra.Composer.MyAccount.Providers
{
    /// <summary>
    /// For Unit test purposes
    /// This has the exact same signature as the statis Membership class
    /// and is intended for easily mocking things
    /// </summary>
    internal interface IMembershipProxy
    {
        MembershipUser GetUser(string userName, bool userIsOnline);
        MembershipUser GetUserByEmail(string userName, bool userIsOnline);
        bool LoginUser(string userName, string password);
        MembershipProviderCollection Providers { get; }
        MembershipProvider Provider { get; }
    }
}
