using FizzWare.NBuilder.Generators;
using Moq;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;

namespace Orckestra.Composer.MyAccount.Tests.Mock
{
    internal static class MockMyAccountUrlProviderFactory
    {
        public static Mock<IMyAccountUrlProvider> Create()
        {
            var myAccountUrlProvider = new Mock<IMyAccountUrlProvider>(MockBehavior.Strict);

            var changePasswordUrl = GetRandom.WwwUrl();
            var createAccountUrl = GetRandom.WwwUrl();
            var forgotPasswordUrl = GetRandom.WwwUrl();
            var loginUrl = GetRandom.WwwUrl();
            var myAccountUrl = GetRandom.WwwUrl();
            var newPasswordUrl = GetRandom.WwwUrl();
            var termsAndConditionsUrl = GetRandom.WwwUrl();
            var addressListUrl = GetRandom.WwwUrl();
            var addAddressUrl = GetRandom.WwwUrl();
            var updateAddressUrl = GetRandom.WwwUrl();

            myAccountUrlProvider
                .Setup(p => p.GetChangePasswordUrl(It.IsAny<GetMyAccountUrlParam>()))
                .Returns(changePasswordUrl);

            myAccountUrlProvider
                .Setup(p => p.GetCreateAccountUrl(It.IsAny<GetMyAccountUrlParam>()))
                .Returns(createAccountUrl);

            myAccountUrlProvider
                .Setup(p => p.GetForgotPasswordUrl(It.IsAny<GetMyAccountUrlParam>()))
                .Returns(forgotPasswordUrl);

            myAccountUrlProvider
                .Setup(p => p.GetLoginUrl(It.IsAny<GetMyAccountUrlParam>()))
                .Returns(loginUrl);

            myAccountUrlProvider
                .Setup(p => p.GetMyAccountUrl(It.IsAny<GetMyAccountUrlParam>()))
                .Returns(myAccountUrl);

            myAccountUrlProvider
                .Setup(p => p.GetAddressListUrl(It.IsAny<GetMyAccountUrlParam>()))
                .Returns(addressListUrl);

            myAccountUrlProvider
                .Setup(p => p.GetAddAddressUrl(It.IsAny<GetMyAccountUrlParam>()))
                .Returns(addAddressUrl);

            myAccountUrlProvider
             .Setup(p => p.GetUpdateAddressBaseUrl(It.IsAny<GetMyAccountUrlParam>()))
             .Returns(updateAddressUrl);

            myAccountUrlProvider
                .Setup(p => p.GetNewPasswordUrl(It.IsAny<GetMyAccountUrlParam>()))
                .Returns(newPasswordUrl);

            myAccountUrlProvider
                .Setup(p => p.GetTermsAndConditionsUrl(It.IsAny<GetMyAccountUrlParam>()))
                .Returns(termsAndConditionsUrl);

            return myAccountUrlProvider;
        }
    }
}
