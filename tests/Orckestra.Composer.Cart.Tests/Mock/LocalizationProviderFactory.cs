using Moq;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;

namespace Orckestra.Composer.Cart.Tests.Mock
{
    internal static class LocalizationProviderFactory
    {
        internal static Mock<ILocalizationProvider> Create()
        {
            var localizationProviderMock = new Mock<ILocalizationProvider>();

            localizationProviderMock
                .Setup(c => c.GetLocalizedString(It.IsAny<GetLocalizedParam>()))
                .Returns("{0}");

            return localizationProviderMock;
        }
    }
}
