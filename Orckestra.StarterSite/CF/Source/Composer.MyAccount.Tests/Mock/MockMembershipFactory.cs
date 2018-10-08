using System.Web.Security;
using Moq;
using Orckestra.Composer.MyAccount.Providers;

namespace Orckestra.Composer.MyAccount.Tests.Mock
{
    internal static class MockMembershipFactory
    {
        /// <summary>
        /// Get the default mocked Membership Static Proxy
        /// </summary>
        /// <returns></returns>
        public static Mock<IMembershipProxy> Create(MembershipProvider provider)
        {
            var mockMembership = new Mock<IMembershipProxy>(MockBehavior.Strict);
            var providers = new MembershipProviderCollection {provider};

            mockMembership.SetupGet(m => m.Provider).Returns(provider);
            mockMembership.SetupGet(m => m.Providers).Returns(providers);

            return mockMembership;
        }
    }
}
