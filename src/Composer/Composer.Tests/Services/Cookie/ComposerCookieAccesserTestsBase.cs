using System.Web.Mvc;
using Autofac.Extras.Moq;
using Autofac.Integration.Mvc;
using FizzWare.NBuilder.Generators;
using Http.TestLibrary;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.ExperienceManagement.Configuration.Settings;

namespace Orckestra.Composer.Tests.Services.Cookie
{
    public abstract class ComposerCookieAccesserTestsBase
    {
        private AutoMock _container;
        private HttpSimulator _httpSimulator;

        public void DependencyResolverSetup()
        {
            _container = AutoMock.GetLoose();

            var cookiesSettingsMoq = _container.Mock<ICookieAccesserSettings>();
            cookiesSettingsMoq.Setup(q => q.Domain).Returns(GetRandom.String(10));
            cookiesSettingsMoq.Setup(q => q.Name).Returns(GetRandom.String(10));
            cookiesSettingsMoq.Setup(q => q.RequireSsl).Returns(false);
            cookiesSettingsMoq.Setup(q => q.TimeoutInMinutes).Returns(10);
            
            var siteConfigMoq = _container.Mock<ISiteConfiguration>();
            siteConfigMoq.Setup(q => q.CookieAccesserSettings).Returns(cookiesSettingsMoq.Object);
            _container.Provide(siteConfigMoq.Object);

            DependencyResolver.SetResolver(new AutofacDependencyResolver(_container.Container));
            _httpSimulator = new HttpSimulator().SimulateRequest();
        }

        public void DependencyResolverTearDown()
        {
            _httpSimulator.Dispose();
            _container.Dispose();
        }

    };
}