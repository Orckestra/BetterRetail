using System;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web.Mvc;
using Autofac;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using HandlebarsDotNet;
using Moq;
using NUnit.Framework;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Lookup;
using Orckestra.Composer.Tests.Mock;
using Orckestra.Composer.ViewEngine;

namespace Orckestra.Composer.Tests.ComposerHost
{
    // ReSharper disable once InconsistentNaming
    [TestFixture]
    internal sealed class ComposerHost_RegisterHandlerbarsHelper
    {
        private IComposerHost _composerHost;
        private ServiceLocatorMoq _serviceLocatorMoq;

        [SetUp]
        public void SetUp()
        {
            // Reset static properties
            typeof(Composer.ComposerHost)
                .GetProperty("Current")
                .GetSetMethod(true)
                .Invoke(null, new object[] { null });

            //Reset Handlerbars static state
            Handlebars.Configuration.Helpers.Clear();
            Handlebars.Configuration.BlockHelpers.Clear();


            // Default setup for all required entities
            Composer.ComposerHost host = new Composer.ComposerHost(Assembly.GetExecutingAssembly());

            // Mock AspNetConfigManager
            Mock<IAspNetConfigurationManager> aspNetConfigManagerMock = new Mock<IAspNetConfigurationManager>(MockBehavior.Strict);
            aspNetConfigManagerMock
                .Setup(config => config.Configure(It.IsAny<ILifetimeScope>(), It.IsAny<IViewEngine>(), It.IsAny<MediaTypeFormatter>()));
            host.SetAspNetConfigurationManager(aspNetConfigManagerMock.Object);

            // Mock AssemblyHelper
            var assemblyHelper = new Mock<Composer.AssemblyHelper>();
            assemblyHelper.Setup(helper => helper.SafeLoadAssemblies(It.IsAny<string>()))
                .Returns(() => new _Assembly[0]);
            host.SetAssemblyHelper(assemblyHelper.Object);

            // Mock Composer Environment
            var composerEnvironment = ComposerEnvironmentFactory.Create();
            host.SetComposerEnvironment(composerEnvironment.Object);

            //Add some additionnal dependencies expected to be set by a plugin (what?!)
            var lookupService = new Mock<ILookupService>(MockBehavior.Strict);
            host.Register<ILookupService>(lookupService.Object);
            var currencyProviderMock = new Mock<ICurrencyProvider>(MockBehavior.Strict);
            host.Register<ICurrencyProvider>(currencyProviderMock.Object);
            _composerHost = host;
            _serviceLocatorMoq = new ServiceLocatorMoq();
        }

        [TearDown]
        public void TearDown()
        {
            _serviceLocatorMoq.Dispose();
        }


        [Test]
        public void WHEN_Passing_Helper_Before_ComposerHost_Init_Completed_SHOULD_Throw_InvalidOperationException()
        {
            //Arrange
            Mock<IHandlebarsHelper> helper = CreateHelper(GetRandom.String(32));

            //Act
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                _composerHost.RegisterHandlebarsHelper(helper.Object);
            });
            
            //Assert
            exception.Message.Should().Contain("Host must be initialized", "Because the helpers must not be registered before the HandlebarsEngine is created.");
        }

        [Test]
        public void WHEN_Passing_BlockHelper_Before_ComposerHost_Init_Completed_SHOULD_Throw_InvalidOperationException()
        {
            //Arrange
            Mock<IHandlebarsBlockHelper> helper = CreateBlockHelper(GetRandom.String(32));

            //Act
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                _composerHost.RegisterHandlebarsHelper(helper.Object);
            });

            //Assert
            exception.Message.Should().Contain("Host must be initialized", "Because the helpers must not be registered before the HandlebarsEngine is created.");
        }

        [Test]
        public void WHEN_Passing_BlockHelper_After_ComposerHost_Init_Completed_SHOULD_Register_To_ViewEngine()
        {
            //Arrange
            string helperName = GetRandom.String(32);
            Mock<IHandlebarsBlockHelper> helper = CreateBlockHelper(helperName);

            //Act
            _composerHost.Init();
            _composerHost.RegisterHandlebarsHelper(helper.Object);

            //Assert
            Handlebars.Configuration.BlockHelpers.Should().ContainKey(helperName);
            Handlebars.Configuration.Helpers.Should().NotContainKey(helperName);
        }

        [Test]
        public void WHEN_Passing_Helper_After_ComposerHost_Init_Completed_SHOULD_Register_To_ViewEngine()
        {
            //Arrange
            string helperName = GetRandom.String(32);
            Mock<IHandlebarsHelper> helper = CreateHelper(helperName);

            //Act
            _composerHost.Init();
            _composerHost.RegisterHandlebarsHelper(helper.Object);

            //Assert
            Handlebars.Configuration.Helpers.Should().ContainKey(helperName);
            Handlebars.Configuration.BlockHelpers.Should().NotContainKey(helperName);
        }

        [Test]
        public void WHEN_Passing_An_HelperName_Already_Registered_For_A_BlockHelper_SHOULD_Throw_InvalidOperationException()
        {
            //Arrange
            string sameName = GetRandom.String(32);
            Mock<IHandlebarsBlockHelper> helperA = CreateBlockHelper(sameName);
            Mock<IHandlebarsBlockHelper> helperB = CreateBlockHelper(sameName);
            Mock<IHandlebarsHelper>      helperC = CreateHelper(sameName);

            //Act
            _composerHost.Init();
            _composerHost.RegisterHandlebarsHelper(helperA.Object);
            var exceptionB = Assert.Throws<InvalidOperationException>(() =>
            {
                _composerHost.RegisterHandlebarsHelper(helperB.Object);
            });
            var exceptionC = Assert.Throws<InvalidOperationException>(() =>
            {
                _composerHost.RegisterHandlebarsHelper(helperC.Object);
            });

            //Assert
            exceptionB.Message.Should().Contain("override not allowed");
            exceptionB.Message.Should().Contain(sameName);

            exceptionC.Message.Should().Contain("override not allowed");
            exceptionC.Message.Should().Contain(sameName);
        }

        [Test]
        public void WHEN_Passing_An_HelperName_Already_Registered_For_A_Helper_SHOULD_Throw_InvalidOperationException()
        {
            //Arrange
            string sameName = GetRandom.String(32);
            Mock<IHandlebarsHelper>      helperA = CreateHelper(sameName);
            Mock<IHandlebarsHelper>      helperB = CreateHelper(sameName);
            Mock<IHandlebarsBlockHelper> helperC = CreateBlockHelper(sameName);

            //Act
            _composerHost.Init();
            _composerHost.RegisterHandlebarsHelper(helperA.Object);
            var exceptionB = Assert.Throws<InvalidOperationException>(() =>
            {
                _composerHost.RegisterHandlebarsHelper(helperB.Object);
            });
            var exceptionC = Assert.Throws<InvalidOperationException>(() =>
            {
                _composerHost.RegisterHandlebarsHelper(helperC.Object);
            });

            //Assert
            exceptionB.Message.Should().Contain("override not allowed");
            exceptionB.Message.Should().Contain(sameName);

            exceptionC.Message.Should().Contain("override not allowed");
            exceptionC.Message.Should().Contain(sameName);
        }
        #region Mock

        private Mock<IHandlebarsBlockHelper> CreateBlockHelper(string helperName)
        {
            Mock<IHandlebarsBlockHelper> helper = new Mock<IHandlebarsBlockHelper>(MockBehavior.Strict);

            helper.SetupGet( h => h.HelperName )
                  .Returns(helperName);

            return helper;
        }

        private Mock<IHandlebarsHelper> CreateHelper(string helperName)
        {
            Mock<IHandlebarsHelper> helper = new Mock<IHandlebarsHelper>(MockBehavior.Strict);

            helper.SetupGet(h => h.HelperName)
                  .Returns(helperName);

            return helper;
        }
        #endregion
    }
}
