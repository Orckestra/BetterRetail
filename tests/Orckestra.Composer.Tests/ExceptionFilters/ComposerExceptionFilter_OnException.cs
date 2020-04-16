using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.ExceptionFilters;
using Orckestra.Composer.Exceptions;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Tests.ExceptionFilters
{
    [TestFixture]
    public class ComposerExceptionFilter_OnException
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            var contextStub = new Mock<IComposerContext>();
            contextStub.SetupGet(mock => mock.CultureInfo).Returns(new CultureInfo("en-CA"));

            var localizationProviderStub = new Mock<ILocalizationProvider>();
            localizationProviderStub.Setup(mock => mock.GetLocalizedString(It.IsAny<GetLocalizedParam>())).Returns(GetRandom.String(5));

            _container = new AutoMocker();
            _container.Use(contextStub);
            _container.Use(localizationProviderStub);
        }

        [Test]
        public void WHEN_executed_context_is_null_SHOULD_throw_argument_null_exception()
        {
            // Arrange
            ComposerExceptionFilter filter = _container.CreateInstance<ComposerExceptionFilter>();

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => filter.OnException(null));
        }

        [Test]
        public void WHEN_executed_context_does_not_contain_composer_exception_SHOULD_not_set_context_response()
        {
            // Arrange
            ComposerExceptionFilter filter = _container.CreateInstance<ComposerExceptionFilter>();

            var context = new HttpActionExecutedContext
            {
                Exception = new InvalidOperationException(),
                ActionContext = new HttpActionContext() // required or setting the context response will throw exception...
            };

            // Act
            filter.OnException(context);

            // Assert
            context.Response.Should().BeNull();
        }

        [Test]
        public void WHEN_executed_context_contains_composer_exception_without_error_SHOULD_not_set_context_response()
        {
            // Arrange
            ComposerExceptionFilter filter = _container.CreateInstance<ComposerExceptionFilter>();

            var errors = new List<ErrorViewModel>();

            var context = new HttpActionExecutedContext
            {
                Exception = new ComposerException(errors),
                ActionContext = new HttpActionContext() // required or setting the context response will throw exception...
            };

            // Act
            filter.OnException(context);

            // Assert
            context.Response.Should().BeNull();
        }

        [Test]
        public void WHEN_executed_context_contains_aggregate_exception_with_composer_exceptions_only_SHOULD_set_invalid_server_error_status_code_to_context_response()
        {
            // Arrange
            ComposerExceptionFilter filter = _container.CreateInstance<ComposerExceptionFilter>();

            var errors = new List<ErrorViewModel>
            {
                new ErrorViewModel
                {
                    ErrorCode = GetRandom.String(1),
                    ErrorMessage = GetRandom.String(1)
                }
            };

            var context = new HttpActionExecutedContext
            {
                Exception = new ComposerException(errors),
                ActionContext = new HttpActionContext() // required or setting the context response will throw exception...
            };

            // Act
            filter.OnException(context);

            // Assert
            context.Response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}
