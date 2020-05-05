using System;
using System.Globalization;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Composer.MyAccount.Tests.Mock;
using Orckestra.ForTests;
using Orckestra.ForTests.Mock;

namespace Orckestra.Composer.MyAccount.Tests.Services
{
    // ReSharper disable once InconsistentNaming
    class MembershipViewService_ChangePasswordAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<ICustomerRepository>(MockBehavior.Strict));
            _container.Use(MockViewModelMapperFactory.Create(typeof(ChangePasswordViewModel).Assembly));
        }

        [Test]
        public async Task WHEN_valid_request_SHOULD_succeed()
        {
            //Arrange
            var expectedCustomer = MockCustomerFactory.CreateRandom();
            var expectedOldPassword = GetRandom.String(32);
            var membershipService = _container.CreateInstance<MembershipViewService>();

            _container.GetMock<ICustomerRepository>()
                .Setup(r => r.GetCustomerByIdAsync(It.Is<GetCustomerByIdParam>(param => param.CustomerId == expectedCustomer.Id)))
                .ReturnsAsync(expectedCustomer);

            _container.GetMock<ICustomerRepository>()
                      .Setup(r => r.ChangePasswordAsync(
                          It.Is<string>(username => username == expectedCustomer.Username),
                          It.IsAny<string>(),
                          It.Is<string>(oldPassword => oldPassword == expectedOldPassword),
                          It.IsAny<string>()))
                      .Returns(Task.FromResult(false));

            //Act
            var result = await membershipService.ChangePasswordAsync(new ChangePasswordParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = expectedCustomer.Id,
                Scope = GetRandom.String(32),
                NewPassword = GetRandom.String(32),
                OldPassword = expectedOldPassword
            });

            //Assert
            result.Status.Should().Be(MyAccountStatus.Success.ToString());
            result.FirstName.Should().Be(expectedCustomer.FirstName);
        }

        [Test]
        public async Task WHEN_user_does_not_exists_SHOULD_fail()
        {
            //Arrange
            var expectedCustomerId = GetRandom.Guid();
            var membershipService = _container.CreateInstance<MembershipViewService>();

            _container.GetMock<ICustomerRepository>()
                .Setup(r => r.GetCustomerByIdAsync(It.Is<GetCustomerByIdParam>(param => param.CustomerId == expectedCustomerId)))
                .ReturnsAsync(null);

            //Act
            var result = await membershipService.ChangePasswordAsync(new ChangePasswordParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = expectedCustomerId,
                Scope = GetRandom.String(32),
                NewPassword = GetRandom.String(32),
                OldPassword = GetRandom.String(32)
            });

            //Assert
            result.Status.Should().Be(MyAccountStatus.Failed.ToString());
        }

        [Test]
        public void WHEN_Param_is_Null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var membershipService = _container.CreateInstance<MembershipViewService>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => membershipService.ChangePasswordAsync(null));

            //Assert
            ex.Message.Should().ContainEquivalentOf("param");
        }

        [Test]
        [TestCase(null)]
        public void WHEN_CultureInfo_is_Null_SHOULD_throw_ArgumentException(CultureInfo cultureInfo)
        {
            //Arrange
            var membershipService = _container.CreateInstance<MembershipViewService>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => membershipService.ChangePasswordAsync(
                new ChangePasswordParam
                {
                    CultureInfo = cultureInfo,
                    CustomerId = GetRandom.Guid(),
                    Scope = GetRandom.String(32),
                    NewPassword = GetRandom.String(32),
                    OldPassword = GetRandom.String(32)
                }
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("CultureInfo");
        }

        [Test]
        public void WHEN_CustomerId_is_Empty_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var customerId = Guid.Empty;
            var membershipService = _container.CreateInstance<MembershipViewService>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => membershipService.ChangePasswordAsync(
                new ChangePasswordParam
                {
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    CustomerId = customerId,
                    Scope = GetRandom.String(32),
                    NewPassword = GetRandom.String(32),
                    OldPassword = GetRandom.String(32)
                }
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("CustomerId");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_OldPassword_is_Empty_SHOULD_throw_ArgumentException(string oldPassword)
        {
            //Arrange
            var membershipService = _container.CreateInstance<MembershipViewService>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => membershipService.ChangePasswordAsync(
                new ChangePasswordParam
                {
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    CustomerId = GetRandom.Guid(),
                    Scope = GetRandom.String(32),
                    NewPassword = GetRandom.String(32),
                    OldPassword = oldPassword
                }
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("OldPassword");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_NewPassword_is_Empty_SHOULD_throw_ArgumentException(string newPassword)
        {
            //Arrange
            var membershipService = _container.CreateInstance<MembershipViewService>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => membershipService.ChangePasswordAsync(
                new ChangePasswordParam
                {
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    CustomerId = GetRandom.Guid(),
                    Scope = GetRandom.String(32),
                    NewPassword = newPassword,
                    OldPassword = GetRandom.String(32)
                }
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("NewPassword");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Scope_is_Empty_SHOULD_throw_ArgumentException(string scope)
        {
            //Arrange
            var membershipService = _container.CreateInstance<MembershipViewService>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => membershipService.ChangePasswordAsync(
                new ChangePasswordParam
                {
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    CustomerId = GetRandom.Guid(),
                    Scope = scope,
                    NewPassword = GetRandom.String(32),
                    OldPassword = GetRandom.String(32)
                }
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("Scope");
        }
    }
}
