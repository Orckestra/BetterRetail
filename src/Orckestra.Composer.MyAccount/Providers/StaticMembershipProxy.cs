using System.Web.Security;

namespace Orckestra.Composer.MyAccount.Providers
{
    /// <summary>
    /// For unit test purposes
    /// </summary>
    internal sealed class StaticMembershipProxy : IMembershipProxy
    {
        public MembershipUser GetUser(string userName, bool userIsOnline)
        {
            return Membership.GetUser(userName, userIsOnline);
        }
        public MembershipUser GetUserByEmail(string email, bool userIsOnline)
        {
            var userName = Membership.GetUserNameByEmail(email);
            return Membership.GetUser(userName, userIsOnline);
        }

        public bool LoginUser(string userName, string password)
        {
            return Membership.ValidateUser(userName, password);
        }

        public MembershipProviderCollection Providers
        {
            get { return Membership.Providers; }
        }

        public MembershipProvider Provider
        {
            get { return Membership.Provider; }
        }
    }
}
