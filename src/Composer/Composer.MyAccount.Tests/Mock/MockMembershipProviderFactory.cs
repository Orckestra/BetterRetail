using System.Web.Security;
using FizzWare.NBuilder.Generators;
using Moq;

namespace Orckestra.Composer.MyAccount.Tests.Mock
{
    internal static class MockMembershipProviderFactory
    {
        /// <summary>
        /// Get the default mocked Membership Provider
        /// </summary>
        /// <returns></returns>
        public static Mock<MembershipProvider> Create()
        {
            var randomName = GetRandom.String(32);
            var provider = new Mock<MembershipProvider>(MockBehavior.Strict);

            provider.SetupGet(m => m.Name).Returns(randomName);

            return provider;
        }
    }
}
