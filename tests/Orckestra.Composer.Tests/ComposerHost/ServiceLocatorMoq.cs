using System;
using System.Reflection;
using Composite.Core;
using FizzWare.NBuilder.Generators;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Orckestra.ExperienceManagement.Configuration.Settings;

namespace Orckestra.Composer.Tests.ComposerHost
{
    public class ServiceLocatorMoq : IDisposable
    {
        public Mock<IServiceProvider> ServiceProviderMoq { get; }
        private readonly Func<IServiceCollection, IServiceProvider> _defaultProviderBuilder;

        public ServiceLocatorMoq()
        {
            var field = typeof(ServiceLocator).GetField("_serviceProviderBuilder", BindingFlags.Static | BindingFlags.NonPublic);
            _defaultProviderBuilder = field.GetValue(null) as Func<IServiceCollection, IServiceProvider>;

            ServiceProviderMoq = new Mock<IServiceProvider>();
            var overtureSettings = new Mock<IOvertureSettings>();

            overtureSettings
                .Setup(q => q.AuthToken)
                .Returns(GetRandom.String(10));

            overtureSettings
                .Setup(q => q.ServicesHostname)
                .Returns(GetRandom.String(10));

            overtureSettings
                .Setup(q => q.Url)
                .Returns(GetRandom.String(10));

            ServiceProviderMoq
                .Setup(x => x.GetService(typeof(IOvertureSettings)))
                .Returns(overtureSettings.Object);

            ServiceLocator.SetServiceProvider(_ => ServiceProviderMoq.Object);
            BuildServiceProvider();
        }

        private void BuildServiceProvider()
        {
            var method = typeof(ServiceLocator).GetMethod("BuildServiceProvider", BindingFlags.Static | BindingFlags.NonPublic);
            method.Invoke(null, new object[0]);
        }

        public void Dispose()
        {
            var field = typeof(ServiceLocator).GetField("_serviceProvider", BindingFlags.Static | BindingFlags.NonPublic);
            field.SetValue(null, null);
            
            ServiceLocator.SetServiceProvider(_defaultProviderBuilder);
        }
    };
}