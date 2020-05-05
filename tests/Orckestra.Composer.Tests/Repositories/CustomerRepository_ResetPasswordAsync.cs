using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Exceptions;
using Orckestra.Composer.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Customers.Membership;
using Orckestra.Overture.ServiceModel.Requests.Customers.Membership;

namespace Orckestra.Composer.Tests.Repositories
{
    // ReSharper disable once InconsistentNaming
    class CustomerRepository_ResetPasswordAsync
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
            var expectedUsername = GetRandom.Email();
            var expectedScope = GetRandom.String(32);
            var expectedPassword = GetRandom.String(32);
            var expectedPasswordAnswer = GetRandom.String(70);
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            _container.GetMock<IOvertureClient>()
                .Setup(r => r.SendAsync(It.Is<ResetPasswordRequest>(
                    param => param.Username == expectedUsername &&
                    param.Password == expectedPassword &&
                    param.PasswordAnswer == expectedPasswordAnswer &&
                    string.IsNullOrWhiteSpace(param.Email))))
                .ReturnsAsync(new ResetPasswordResponse
                {
                    Success = true
                });

            //Act and Assert
            Assert.DoesNotThrowAsync(() => customerRepository.ResetPasswordAsync(expectedUsername, expectedScope, expectedPassword, expectedPasswordAnswer));
        }

        [Test]
        public void WHEN_bad_request_SHOULD_throw_composer_exception()
        {
            //Arrange
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            _container.GetMock<IOvertureClient>()
                .Setup(r => r.SendAsync(It.IsAny<ResetPasswordRequest>()))
                .ReturnsAsync(new ResetPasswordResponse
                {
                    Success = false
                });

            //Act
            Assert.ThrowsAsync<ComposerException>(() => customerRepository.ResetPasswordAsync(
                GetRandom.Email(),
                GetRandom.String(32),
                GetRandom.String(32),
                GetRandom.String(70)
            ));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Username_is_Empty_SHOULD_throw_ArgumentException(string username)
        {
            //Arrange
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerRepository.ResetPasswordAsync(
                username,
                GetRandom.String(32),
                GetRandom.String(32),
                GetRandom.String(70)
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("username");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_NewPassword_is_Empty_SHOULD_throw_ArgumentException(string newPassword)
        {
            //Arrange
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerRepository.ResetPasswordAsync(
                GetRandom.Email(),
                GetRandom.String(70),
                newPassword,
                GetRandom.String(70)
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("newPassword");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_PasswordAnswer_is_Empty_SHOULD_succeed(string passwordAnswer)
        {
            //Arrange
            var expectedUsername = GetRandom.Email();
            var expectedScopeId = GetRandom.String(32);
            var expectedPassword = GetRandom.String(32);
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            _container.GetMock<IOvertureClient>()
                .Setup(r => r.SendAsync(It.Is<ResetPasswordRequest>(
                    param => param.Username == expectedUsername &&
                    param.ScopeId == expectedScopeId &&
                    param.Password == expectedPassword &&
                    param.PasswordAnswer == passwordAnswer &&
                    string.IsNullOrWhiteSpace(param.Email))))
                .ReturnsAsync(new ResetPasswordResponse
                {
                    Success = true
                });

            //Act and Assert
            Assert.DoesNotThrowAsync(() =>
                 customerRepository.ResetPasswordAsync(
                    expectedUsername,
                    expectedScopeId,
                    expectedPassword,
                    passwordAnswer)
            );
        }
    }
}
