using System;
using System.Globalization;
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
using Orckestra.Composer.Tests.Mock;
using Orckestra.ForTests;
using Orckestra.ForTests.Mock;

namespace Orckestra.Composer.MyAccount.Tests.Services
{
    // ReSharper disable once InconsistentNaming
    class MembershipViewService_GetLoginViewModel
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(MockMyAccountUrlProviderFactory.Create());
            _container.Use(MockViewModelMapperFactory.Create(typeof(LoginViewModel).Assembly));
            _container.Use(new Mock<IMembershipProxy>(MockBehavior.Strict));
        }

        [Test]
        public void WHEN_passing_valid_arguments_SHOULD_create_viewmodel()
        {
            //Arrange
            var expectedStatus = TestingExtensions.GetRandomEnum<MyAccountStatus>();
            var param = new GetLoginViewModelParam
            {
                Status = expectedStatus,
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Customer = MockCustomerFactory.CreateRandom(),
                ReturnUrl = GetRandom.WwwUrl(),
                LoginUrl = GetRandom.WwwUrl(),
                CreateAccountUrl = GetRandom.WwwUrl(),
                ForgotPasswordUrl = GetRandom.WwwUrl(),
                Username = GetRandom.String(32)
            };
            var membershipViewService = _container.CreateInstance<MembershipViewService>();
            membershipViewService.Membership = _container.Get<IMembershipProxy>();

            //Act
            var viewModel = membershipViewService.GetLoginViewModel(param);

            //Assert
            viewModel.Should().NotBeNull("This view model should never be null");
            viewModel.Status.Should().Be(expectedStatus.ToString("G"), "Because we render the status as a string for HBS message switch");
            viewModel.ReturnUrl.Should().Be(param.ReturnUrl);
            viewModel.CreateAccountUrl.Should().Be(param.CreateAccountUrl);
            viewModel.ForgotPasswordUrl.Should().Be(param.ForgotPasswordUrl);
            viewModel.LoginUrl.Should().Be(param.LoginUrl);
            viewModel.FirstName.Should().Be(param.Customer.FirstName);
            viewModel.LastName.Should().Be(param.Customer.LastName);
        }

        [Test]
        public void WHEN_Param_is_NULL_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var membershipViewService = _container.CreateInstance<MembershipViewService>();

            //Act
            var ex = Assert.Throws<ArgumentNullException>(() => membershipViewService.GetLoginViewModel(null));

            //Assert
            ex.Message.Should().ContainEquivalentOf("param");
        }

        [Test]
        [TestCase(null)]
        public void WHEN_CultureInfo_is_Empty_SHOULD_throw_ArgumentException(CultureInfo cultureInfo)
        {
            //Arrange
            var membershipViewService = _container.CreateInstance<MembershipViewService>();
            var param = new GetLoginViewModelParam
            {
                Status = TestingExtensions.GetRandomEnum<MyAccountStatus>(),
                CultureInfo = cultureInfo,
                Customer = MockCustomerFactory.CreateRandom(),
                ReturnUrl = GetRandom.WwwUrl()
            };

            //Act
            var ex = Assert.Throws<ArgumentException>(() =>  membershipViewService.GetLoginViewModel(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("CultureInfo");
        }

        [Test]
        public void WHEN_Customer_is_Null_SHOULD_create_view_model_with_empty_bag()
        {
            //Arrange
            var expectedStatus = TestingExtensions.GetRandomEnum<MyAccountStatus>();
            var param = new GetLoginViewModelParam
            {
                Status = expectedStatus,
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Customer = null,
                ReturnUrl = GetRandom.WwwUrl(),
                Username = GetRandom.String(32),
                ForgotPasswordUrl = GetRandom.WwwUrl(),
                CreateAccountUrl = GetRandom.WwwUrl(),
                LoginUrl = GetRandom.WwwUrl(),
            };
            var membershipViewService = _container.CreateInstance<MembershipViewService>();
            membershipViewService.Membership = _container.Get<IMembershipProxy>();

            //Act
            var viewModel = membershipViewService.GetLoginViewModel(param);

            //Assert
            viewModel.Should().NotBeNull("This view model should never be null");
            viewModel.Status.Should().Be(expectedStatus.ToString("G"), "Because we render the status as a string for HBS message switch");
            viewModel.ReturnUrl.Should().Be(param.ReturnUrl);
            viewModel.CreateAccountUrl.Should().Be(param.CreateAccountUrl);
            viewModel.ForgotPasswordUrl.Should().Be(param.ForgotPasswordUrl);
            viewModel.LoginUrl.Should().Be(param.LoginUrl);
            viewModel.Bag.Should().BeEmpty("No PropertyBag to load from");
        }

        [Test]
        [TestCase(null)]
        public void WHEN_Status_is_NULL_SHOULD_create_view_model_with_empty_status(MyAccountStatus? status)
        {
            //Arrange
            var param = new GetLoginViewModelParam
            {
                Status = status,
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Customer = MockCustomerFactory.CreateRandom(),
                ReturnUrl = GetRandom.WwwUrl(),
                Username = GetRandom.String(32)
            };
            var membershipViewService = _container.CreateInstance<MembershipViewService>();

            //Act
            var viewModel = membershipViewService.GetLoginViewModel(param);

            //Assert
            viewModel.Status.Should().BeEmpty();
        }
    }
}
