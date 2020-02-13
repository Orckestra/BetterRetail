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
