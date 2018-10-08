using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Security;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Providers;
using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.MyAccount.Tests.Mock;
using Orckestra.ForTests;

namespace Orckestra.Composer.MyAccount.Tests.Services
{
    // ReSharper disable once InconsistentNaming
    class MembershipViewService_ResetPasswordAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<ICustomerRepository>(MockBehavior.Strict));
            _container.Use(MockMembershipProviderFactory.Create());
            _container.Use(MockMembershipFactory.Create(_container.Get<MembershipProvider>()));

            _container.GetMock<MembershipProvider>()
                      .SetupGet(m => m.MinRequiredPasswordLength)
                      .Returns(GetRandom.Int)
                      .Verifiable();

            _container.GetMock<MembershipProvider>()
                .SetupGet(m => m.MinRequiredNonAlphanumericCharacters)
                .Returns(GetRandom.Int)
                .Verifiable("Regex must be based on this value");
        }

        [Test]
        public async void WHEN_valid_request_SHOULD_succeed()
        {
            //Arrange
            var expectedTicket = GetRandom.String(1024);
            var expectedCustomer = MockCustomerFactory.CreateRandom();
            var membershipService = _container.CreateInstance<MembershipViewService>();

            _container.GetMock<ICustomerRepository>()
                .Setup(r => r.GetCustomerByTicketAsync(It.Is<string>(ticket => ticket == expectedTicket)))
                .ReturnsAsync(expectedCustomer);

            _container.GetMock<ICustomerRepository>()
                .Setup(r => r.ResetPasswordAsync(
                    It.Is<string>(username => username == expectedCustomer.Username),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            //Act
            var result = await membershipService.ResetPasswordAsync(
                new ResetPasswordParam
                {
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    Ticket = expectedTicket,
                    Scope = GetRandom.String(32),
                    NewPassword = GetRandom.String(32),
                    PasswordAnswer = GetRandom.String(70)
                }
            );

            //Assert
            result.Status.Should().Be(MyAccountStatus.Success.ToString());
        }

        [Test]
        public void WHEN_Param_is_Null_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var membershipService = _container.CreateInstance<MembershipViewService>();

            //Act
            var ex = Assert.Throws<ArgumentNullException>(async () => await membershipService.ResetPasswordAsync(null));

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
            var ex = Assert.Throws<ArgumentException>(async () => await membershipService.ResetPasswordAsync(
                new ResetPasswordParam
                {
                    CultureInfo = cultureInfo,
                    Ticket = GetRandom.String(1024),
                    Scope = GetRandom.String(32),
                    NewPassword = GetRandom.String(32),
                    PasswordAnswer = GetRandom.String(70)
                }
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("CultureInfo");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Ticket_is_Empty_SHOULD_throw_ArgumentException(string ticket)
        {
            //Arrange
            var membershipService = _container.CreateInstance<MembershipViewService>();

            //Act
            var ex = Assert.Throws<ArgumentException>(async () => await membershipService.ResetPasswordAsync(
                new ResetPasswordParam
                {
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    Ticket = ticket,
                    Scope = GetRandom.String(32),
                    NewPassword = GetRandom.String(32),
                    PasswordAnswer = GetRandom.String(70)
                }
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("Ticket");
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
            var ex = Assert.Throws<ArgumentException>(async () => await membershipService.ResetPasswordAsync(
                new ResetPasswordParam
                {
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    Ticket = GetRandom.String(1024),
                    Scope = scope,
                    NewPassword = GetRandom.String(32),
                    PasswordAnswer = GetRandom.String(70)
                }
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("Scope");
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
            var ex = Assert.Throws<ArgumentException>(async () => await membershipService.ResetPasswordAsync(
                new ResetPasswordParam
                {
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    Ticket = GetRandom.String(1024),
                    Scope = GetRandom.String(32),
                    NewPassword = newPassword,
                    PasswordAnswer = GetRandom.String(70)
                }
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("NewPassword");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public void WHEN_Membership_Refuses_Empty_PasswordAnsnwer_SHOULD_throw_ArgumentException(string passwordAnswer)
        {
            //Arrange
            var membershipService = _container.CreateInstance<MembershipViewService>();

            //Act
            var ex = Assert.Throws<ArgumentException>(async () => await membershipService.ResetPasswordAsync(
                new ResetPasswordParam
                {
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    Ticket = GetRandom.String(1024),
                    Scope = GetRandom.String(32),
                    NewPassword = GetRandom.String(32),
                    PasswordAnswer = passwordAnswer
                }
            ));

            //Assert
            ex.Message.Should().ContainEquivalentOf("PasswordAnswer");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" \t\r\n")]
        public async void WHEN_Membership_Accept_Empty_PasswordAnsnwer_SHOULD_succeed(string passwordAnswer)
        {
            //Arrange
            var membershipService = _container.CreateInstance<MembershipViewService>();
            membershipService.Membership = _container.Get<IMembershipProxy>();

            _container.GetMock<MembershipProvider>()
                .SetupGet(p => p.RequiresQuestionAndAnswer)
                .Returns(false);

            _container.GetMock<ICustomerRepository>()
                .Setup(r => r.GetCustomerByTicketAsync(It.IsAny<string>()))
                .ReturnsAsync(MockCustomerFactory.CreateRandom());

            _container.GetMock<ICustomerRepository>()
                .Setup(r => r.ResetPasswordAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.Is<string>(pa => pa == passwordAnswer)))
                .Returns(Task.FromResult(false));

            //Act
            var result = await membershipService.ResetPasswordAsync(
                new ResetPasswordParam
                {
                    CultureInfo = TestingExtensions.GetRandomCulture(),
                    Ticket = GetRandom.String(1024),
                    Scope = GetRandom.String(32),
                    NewPassword = GetRandom.String(32),
                    PasswordAnswer = passwordAnswer
                }
            );

            //Assert
            result.Status.Should().Be(MyAccountStatus.Success.ToString());
        }
    }
}
