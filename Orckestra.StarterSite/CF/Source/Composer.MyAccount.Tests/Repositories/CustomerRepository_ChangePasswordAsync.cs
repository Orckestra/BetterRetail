using System;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Exceptions;
using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Customers.Membership;
using Orckestra.Overture.ServiceModel.Requests.Customers.Membership;

namespace Orckestra.Composer.MyAccount.Tests.Repositories
{
    // ReSharper disable once InconsistentNaming
    class CustomerRepository_ChangePasswordAsync
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
            var expectedOldPassword = GetRandom.String(32);
            var expectedNewPassword = GetRandom.String(32);
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            _container.GetMock<IOvertureClient>()
                .Setup(r => r.SendAsync(It.Is<ChangePasswordRequest>(
                    param => param.UserName == expectedUsername &&
                    param.NewPassword == expectedNewPassword &&
                    param.OldPassword == expectedOldPassword)))
                .ReturnsAsync(new ChangePasswordResponse
                {
                    Success = true
                });

            //Act and Assert
            Assert.DoesNotThrow(async () =>
                await customerRepository.ChangePasswordAsync(expectedUsername,
                expectedScope,
                expectedOldPassword, 
                expectedNewPassword));
        }

        [Test]
        public void WHEN_invalid_request_SHOULD_throw_composer_exception()
        {
            //Arrange
            var expectedUsername = GetRandom.Email();
            var expectedScope = GetRandom.String(32);
            var expectedOldPassword = GetRandom.String(32);
            var expectedNewPassword = GetRandom.String(32);
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            _container.GetMock<IOvertureClient>()
                .Setup(r => r.SendAsync(It.Is<ChangePasswordRequest>(
                    param => param.UserName == expectedUsername &&
                    param.NewPassword == expectedNewPassword &&
                    param.OldPassword == expectedOldPassword)))
                .ReturnsAsync(new ChangePasswordResponse
                {
                    Success = false
                });

            //Act and Assert
            Assert.Throws<ComposerException>(async () =>
                await customerRepository.ChangePasswordAsync(expectedUsername,
                expectedScope,
                expectedOldPassword, 
                expectedNewPassword));
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
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerRepository.ChangePasswordAsync(
                username,
                GetRandom.String(32),
                GetRandom.String(32),
                GetRandom.String(32)
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("username");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_OldPassword_is_Empty_SHOULD_throw_ArgumentException(string oldPassword)
        {
            //Arrange
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerRepository.ChangePasswordAsync(
                GetRandom.Email(),
                GetRandom.String(32),
                oldPassword,
                GetRandom.String(32)
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("oldPassword");
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
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerRepository.ChangePasswordAsync(
                GetRandom.String(32),
                GetRandom.Email(),
                GetRandom.String(32),
                newPassword
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("newPassword");
        }
    }
}
