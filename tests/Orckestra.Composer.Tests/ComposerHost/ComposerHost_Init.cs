using System;
using System.Linq.Expressions;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web.Mvc;
using Autofac;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.Tests.Mock;
using Orckestra.ExperienceManagement.Configuration;

namespace Orckestra.Composer.Tests.ComposerHost
{
    [TestFixture]
    public class ComposerHostInit
    {
        private Composer.ComposerHost _composerHost;
        private Mock<IAspNetConfigurationManager> _aspNetConfigManagerMock;

        private readonly Expression<Action<IAspNetConfigurationManager>> _configureCall = config =>
                config.Configure(It.IsAny<ILifetimeScope>(),
                It.IsAny<IViewEngine>(),
                It.IsAny<MediaTypeFormatter>());

        private ServiceLocatorMoq _serviceLocatorMoq;

        [SetUp]
        public void SetUp()
        {
            // Reset static properties
            typeof(Composer.ComposerHost)
                .GetProperty("Current")
                .GetSetMethod(true)
                .Invoke(null, new object[] { null });

            TestPlugin.WasDiscovered = false;

            // Default setup for all required entities
            _composerHost = new Composer.ComposerHost(Assembly.GetExecutingAssembly());

            // Mock AspNetConfigManager
            _aspNetConfigManagerMock = new Mock<IAspNetConfigurationManager>();
            _aspNetConfigManagerMock.Setup(_configureCall);
            _composerHost.SetAspNetConfigurationManager(_aspNetConfigManagerMock.Object);

            // Mock AssemblyHelper
            var assemblyHelper = new Mock<Composer.AssemblyHelper>();
            assemblyHelper.Setup(helper => helper.SafeLoadAssemblies(It.IsAny<string>()))
                .Returns(() => new _Assembly[0]);
            _composerHost.SetAssemblyHelper(assemblyHelper.Object);

            // Mock Composer Environment
            var composerEnvironment = ComposerEnvironmentFactory.Create();
            _composerHost.SetComposerEnvironment(composerEnvironment.Object);

            //Add some additional dependencies expected to be set by a plugin (what?!)
            var lookupService = new Mock<ILookupService>(MockBehavior.Strict);
            _composerHost.Register<ILookupService>(lookupService.Object);
            var currencySettingService = new Mock<ICurrencyProvider>(MockBehavior.Strict);
            _composerHost.Register<ICurrencyProvider>(currencySettingService.Object);
            _serviceLocatorMoq = new ServiceLocatorMoq();
        }

        [TearDown]
        public void TearDown()
        {
            _serviceLocatorMoq.Dispose();
        }

        [Test]
        public void WHEN_Everything_Is_Good_SHOULD_Init_Successfully()
        {
            // Arrange
            _composerHost.MonitorEvents();

            // Act
            _composerHost.Init();

            // Assert
            _composerHost.ShouldRaise("Initialized");
            _aspNetConfigManagerMock.Verify(_configureCall);
            TestPlugin.WasDiscovered.Should().BeTrue();
        }

        [Test]
        public void WHEN_Already_Init_SHOULD_Throw_InvalidOperationException()
        {
            // Arrange
            _composerHost.Init();

            // Act
            Action action = () => _composerHost.Init();

            // Assert
            action.ShouldThrow<InvalidOperationException>();
        }
    }
}
