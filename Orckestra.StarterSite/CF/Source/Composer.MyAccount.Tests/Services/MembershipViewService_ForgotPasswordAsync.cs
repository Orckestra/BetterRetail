using System;
using System.Globalization;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Exceptions;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.ForTests;

namespace Orckestra.Composer.MyAccount.Tests.Services
{
    // ReSharper disable once InconsistentNaming
    class MembershipViewService_ForgotPasswordAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<ICustomerRepository>(MockBehavior.Strict));
        }

        [Test]
        public async void WHEN_valid_request_SHOULD_succeed()
        {
            //Arrange
            var expectedEmail = GetRandom.Email();
            var membershipService = _container.CreateInstance<MembershipViewService>();

            _container.GetMock<ICustomerRepository>()
                .Setup(r => r.SendResetPasswordInstructionsAsync(It.Is<SendResetPasswordInstructionsParam>(p => p.Email == expectedEmail)))
                .Returns(Task.FromResult(false));

            //Act
            var result = await membershipService.ForgotPasswordAsync(new ForgotPasswordParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Scope = GetRandom.String(32),
                Email = expectedEmail
            });

            //Assert
            result.Status.Should().Be(MyAccountStatus.Success.ToString());
        }

        [Test]
        public async void WHEN_bad_request_SHOULD_succeed()
        {
            //Arrange
            var expectedEmail = GetRandom.Email();
            var membershipService = _container.CreateInstance<MembershipViewService>();

            _container.GetMock<ICustomerRepository>()
                      .Setup(r => r.SendResetPasswordInstructionsAsync(It.Is<SendResetPasswordInstructionsParam>(p => p.Email == expectedEmail)))
                      .Throws(new ComposerException(GetRandom.String(2)));

            //Act
            var result = await membershipService.ForgotPasswordAsync(new ForgotPasswordParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Scope = GetRandom.String(32),
                Email = expectedEmail
            });

            //Assert
            result.Status.Should().Be(MyAccountStatus.Success.ToString(), "For security, the default implementation does not provide any information on the user's existance");
        }

        [Test]
        public void WHEN_Param_is_Null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var membershipService = _container.CreateInstance<MembershipViewService>();

            //Act
            var ex = Assert.Throws<ArgumentNullException>(async () => await membershipService.ForgotPasswordAsync(null));

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
            var ex = Assert.Throws<ArgumentException>(async () => await membershipService.ForgotPasswordAsync(
                new ForgotPasswordParam
                {
                    CultureInfo = cultureInfo,
                    Email = GetRandom.Email(),
                    Scope = GetRandom.String(32)
                }
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("CultureInfo");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Email_is_Empty_SHOULD_throw_ArgumentException(string email)
        {
            //Arrange
            var membershipService = _container.CreateInstance<MembershipViewService>();

            //Act
            var ex = Assert.Throws<ArgumentException>(async () => await membershipService.ForgotPasswordAsync(
                new ForgotPasswordParam
                {
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    Email = email,
                    Scope = GetRandom.String(32)
                }
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("Email");
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
            var ex = Assert.Throws<ArgumentException>(async () => await membershipService.ForgotPasswordAsync(
                new ForgotPasswordParam
                {
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    Email = GetRandom.Email(),
                    Scope = scope
                }
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("Scope");
        }
    }
}
