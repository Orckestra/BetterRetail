using System;
using System.Web.Mvc;
using Autofac.Extras.Moq;
using Autofac.Integration.Mvc;
using Http.TestLibrary;

namespace Orckestra.Composer.Tests.Providers.Membership
{
    public class AutofacDependencyResolverMoq : IDisposable
    {
        public AutoMock AutoMock { get; }
        private readonly HttpSimulator _httpSimulator;

        public AutofacDependencyResolverMoq()
        {
            AutoMock = AutoMock.GetLoose();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(AutoMock.Container));
            _httpSimulator = new HttpSimulator().SimulateRequest();
        }

        public void Dispose()
        {
            _httpSimulator.Dispose();
            AutoMock.Dispose();
        }
    };
}