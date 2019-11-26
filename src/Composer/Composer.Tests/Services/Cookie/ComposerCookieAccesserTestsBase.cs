using FizzWare.NBuilder.Generators;
using Orckestra.Composer.Tests.Providers.Membership;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.ExperienceManagement.Configuration.Settings;

namespace Orckestra.Composer.Tests.Services.Cookie
{
    public abstract class ComposerCookieAccesserTestsBase
    {
        private AutofacDependencyResolverMoq _autofacDependencyResolverMoq;

        public void DependencyResolverSetup()
        {
            _autofacDependencyResolverMoq = new AutofacDependencyResolverMoq();

            var cookiesSettingsMoq = _autofacDependencyResolverMoq.AutoMock.Mock<ICookieAccesserSettings>();
            cookiesSettingsMoq.Setup(q => q.Domain).Returns(GetRandom.String(10));
            cookiesSettingsMoq.Setup(q => q.Name).Returns(GetRandom.String(10));
            cookiesSettingsMoq.Setup(q => q.RequireSsl).Returns(false);
            cookiesSettingsMoq.Setup(q => q.TimeoutInMinutes).Returns(10);
            
            var siteConfigMoq = _autofacDependencyResolverMoq.AutoMock.Mock<ISiteConfiguration>();
            siteConfigMoq.Setup(q => q.CookieAccesserSettings).Returns(cookiesSettingsMoq.Object);
            _autofacDependencyResolverMoq.AutoMock.Provide(siteConfigMoq.Object);
        }

        public void DependencyResolverTearDown()
        {
            _autofacDependencyResolverMoq.Dispose();
            _autofacDependencyResolverMoq = null;
        }

    };
}