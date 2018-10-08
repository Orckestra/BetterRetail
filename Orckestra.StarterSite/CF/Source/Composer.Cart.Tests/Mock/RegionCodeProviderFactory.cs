using Moq;
using Orckestra.Composer.Providers;

namespace Orckestra.Composer.Cart.Tests.Mock
{
    internal static class RegionCodeProviderFactory
    {
        internal static Mock<IRegionCodeProvider> Create()
        {
            var localizationProviderMock = new Mock<IRegionCodeProvider>();

            localizationProviderMock
                .Setup(mock => mock.GetRegion(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("CA");

            return localizationProviderMock;
        }
    }
}
