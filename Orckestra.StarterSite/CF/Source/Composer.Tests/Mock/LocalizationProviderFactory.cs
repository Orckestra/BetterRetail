using Moq;
using Moq.AutoMock;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;

namespace Orckestra.Composer.Tests.Mock
{
    internal static class LocalizationProviderFactory
    {
        public static ILocalizationProvider CreateFromTestAssets()
        {
            AutoMocker container = new AutoMocker();
            container.Use(CacheProviderFactory.CreateForLocalizationTree());
            container.Use(ComposerEnvironmentFactory.Create());

            return container.CreateInstance<ResourceLocalizationProvider>();
        }
        public static Mock<ILocalizationProvider> CreateKnowsNothing()
        {
            Mock<ILocalizationProvider> localizationProvider = new Mock<ILocalizationProvider>(MockBehavior.Strict);

            localizationProvider
                .Setup(lp => lp.GetLocalizedString(It.IsNotNull<GetLocalizedParam>()))
                .Returns<GetLocalizedParam>(null)
                .Verifiable();

            return localizationProvider;
        }
        public static Mock<ILocalizationProvider> CreateKnowsItAll()
        {
            Mock<ILocalizationProvider> localizationProvider = new Mock<ILocalizationProvider>(MockBehavior.Strict);

            localizationProvider
                .Setup(lp => lp.GetLocalizedString(It.IsNotNull<GetLocalizedParam>()))
                .Returns<GetLocalizedParam>(param =>
                {
                    return string.Format("{0}:{1}.{2}", param.CultureInfo, param.Category, param.Key);
                })
                .Verifiable();

            return localizationProvider;
        }


    }
}
