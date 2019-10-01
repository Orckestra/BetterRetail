using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Exceptions;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Customers.Membership;
using Orckestra.Overture.ServiceModel.Requests.Customers.Membership;

namespace Orckestra.Composer.MyAccount.Tests.Repositories
{
    // ReSharper disable once InconsistentNaming
    internal class CustomerRepository_SendResetPasswordInstructionsAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<IOvertureClient>(MockBehavior.Strict));
        }

        [Test]
        public void WHEN_valid_request_SHOULD_succeed()
        {
            //Arrange
            var expectedEmail = GetRandom.Email();

            var customerRepository = _container.CreateInstance<CustomerRepository>();

            _container.GetMock<IOvertureClient>()
                .Setup(r => r.SendAsync(It.Is<ResetPasswordRequest>(
                    param => string.IsNullOrWhiteSpace(param.Username) &&
                             string.IsNullOrWhiteSpace(param.Password) &&
                             string.IsNullOrWhiteSpace(param.PasswordAnswer) &&
                             param.Email == expectedEmail)))
                .ReturnsAsync(new ResetPasswordResponse
                {
                    Success = true
                });

            //Act and Assert
            var sendResetParam = new SendResetPasswordInstructionsParam
            {
                Email = expectedEmail,
                Scope = GetRandom.String(32)
            };

            Assert.DoesNotThrowAsync(() => customerRepository.SendResetPasswordInstructionsAsync(sendResetParam));
        }

        [Test]
        public void WHEN_bad_request_SHOULD_throw_composer_exception()
        {
            //Arrange
            var expectedEmail = GetRandom.Email();
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            _container.GetMock<IOvertureClient>()
                .Setup(r => r.SendAsync(It.Is<ResetPasswordRequest>(
                    param => string.IsNullOrWhiteSpace(param.Username) &&
                             string.IsNullOrWhiteSpace(param.Password) &&
                             string.IsNullOrWhiteSpace(param.PasswordAnswer) &&
                             param.Email == expectedEmail)))
                .ReturnsAsync(new ResetPasswordResponse
                {
                    Success = false
                });

            //Act and Assert
            var sendResetParam = new SendResetPasswordInstructionsParam
            {
                Email = expectedEmail,
                Scope = GetRandom.String(32)
            };

            Assert.ThrowsAsync<ComposerException>(() => customerRepository.SendResetPasswordInstructionsAsync(sendResetParam));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Email_is_Empty_SHOULD_throw_ArgumentException(string email)
        {
            //Arrange
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            //Act  
            var sendResetParam = new SendResetPasswordInstructionsParam
            {
                Email = email,
                Scope = GetRandom.String(32)
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerRepository.SendResetPasswordInstructionsAsync(sendResetParam));

            //Assert
            ex.Message.Should().ContainEquivalentOf("email");
        }
    }
}
