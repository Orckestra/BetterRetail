using System.Web.Security;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Providers;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.MyAccount.Tests.Mock;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.ForTests.Mock;
using System.Text.RegularExpressions;

namespace Orckestra.Composer.MyAccount.Tests.Services
{
    [TestFixture]
    public class MembershipViewServiceGetCreateAccountViewModel
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<IMembershipProxy>(MockBehavior.Strict));
            _container.Use(MockViewModelMapperFactory.Create(typeof (CreateAccountViewModel).Assembly));
            _container.Use(MockMembershipProviderFactory.Create());
            _container.Use(MockMembershipFactory.Create(_container.Get<MembershipProvider>()));

            _container.GetMock<MembershipProvider>()
                .SetupGet(m => m.MinRequiredPasswordLength)
                .Returns(GetRandom.PositiveInt())
                .Verifiable();
        }

        [Test]
        public void WHEN_passing_valid_arguments_SHOULD_create_viewmodel_with_regex()
        {
            // Arrange
            var param = new GetCreateAccountViewModelParam();
            var minNumber = GetRandom.PositiveInt();
            _container.GetMock<MembershipProvider>()
                .SetupGet(m => m.MinRequiredNonAlphanumericCharacters)
                .Returns(minNumber)
                .Verifiable("Regex must be based on this value");

            var membershipViewService = _container.CreateInstance<MembershipViewService>();
            membershipViewService.Membership = _container.Get<IMembershipProxy>();

            // Act
            var viewModel = membershipViewService.GetCreateAccountViewModel(param);

            // Assert
            viewModel.Should().NotBeNull();
            viewModel.PasswordRegexPattern.Should().NotBeNullOrEmpty();
            viewModel.PasswordRegexPattern.Should()
                .Contain(minNumber + "", "the Regex must be based on the min required non alphanumeric characters");
        }

        [Test]
        public void WHEN_min_required_non_alphanumeric_is_specified_SHOULD_validate_password_correctly()
        {
            // Arrange
            var param = new GetCreateAccountViewModelParam();
            var minNumber = 3;

            var goodPassword = "My!GoodPa@ssword!";
            var badPassword = "MyB@dPassword!";

            _container.GetMock<MembershipProvider>()
                .SetupGet(m => m.MinRequiredNonAlphanumericCharacters)
                .Returns(minNumber);                

            var membershipViewService = _container.CreateInstance<MembershipViewService>();
            membershipViewService.Membership = _container.Get<IMembershipProxy>();

            // Act
            var viewModel = membershipViewService.GetCreateAccountViewModel(param);

            // Assert
            viewModel.Should().NotBeNull();
            viewModel.PasswordRegexPattern.Should().NotBeNullOrEmpty();

            Regex.IsMatch(goodPassword, viewModel.PasswordRegexPattern).Should().BeTrue();
            Regex.IsMatch(badPassword, viewModel.PasswordRegexPattern).Should().BeFalse();
        }
    }
}