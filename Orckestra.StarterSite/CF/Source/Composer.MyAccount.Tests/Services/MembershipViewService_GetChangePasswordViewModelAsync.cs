using System;
using System.Globalization;
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
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.ForTests;
using Orckestra.ForTests.Mock;
using System.Threading.Tasks;

namespace Orckestra.Composer.MyAccount.Tests.Services
{
    // ReSharper disable once InconsistentNaming
    class MembershipViewService_GetChangePasswordViewModelAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(MockMyAccountUrlProviderFactory.Create());
            _container.Use(MockViewModelMapperFactory.Create(typeof(ChangePasswordViewModel).Assembly));
            _container.Use(MockMembershipProviderFactory.Create());
            _container.Use(MockMembershipFactory.Create(_container.Get<MembershipProvider>()));
        }

        [Test]
        public async Task WHEN_passing_valid_arguments_SHOULD_create_viewmodel()
        {
            //Arrange
            var customer = MockCustomerFactory.CreateRandom();
            var expectedPasswordLength = GetRandom.PositiveInt();
            var expectedPasswordNonAlpha = GetRandom.PositiveInt();

            var param = new GetCustomerChangePasswordViewModelParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Scope = GetRandom.String(32),
                CustomerId = customer.Id,
            };
            var membershipViewService = _container.CreateInstance<MembershipViewService>();
            membershipViewService.Membership = _container.Get<IMembershipProxy>();

            _container.GetMock<MembershipProvider>()
                .SetupGet(m => m.MinRequiredPasswordLength)
                .Returns(expectedPasswordLength)
                .Verifiable();

            _container.GetMock<MembershipProvider>()
                .SetupGet(m => m.MinRequiredNonAlphanumericCharacters)
                .Returns(expectedPasswordNonAlpha)
                .Verifiable("Regex must be based on this value");

            var customerRepository = new Mock<ICustomerRepository>();
            customerRepository.Setup(r => r.GetCustomerByIdAsync(It.IsAny<GetCustomerByIdParam>()))
                .ReturnsAsync(customer);

            //Act
            var viewModel = await membershipViewService.GetChangePasswordViewModelAsync(param);

            //Assert
            viewModel.Should().NotBeNull("This view model should never be null");
            viewModel.MinRequiredPasswordLength.Should().Be(expectedPasswordLength);
            viewModel.MinRequiredNonAlphanumericCharacters.Should().Be(expectedPasswordNonAlpha);
            viewModel.PasswordRegexPattern.Should().NotBeNull();

            _container.GetMock<IMembershipProxy>().VerifyAll();
        }

        [Test]
        public void WHEN_Param_is_NULL_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var membershipViewService = _container.CreateInstance<MembershipViewService>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => membershipViewService.GetChangePasswordViewModelAsync(null));

            //Assert
            ex.Message.Should().ContainEquivalentOf("param");
        }

        [Test]
        [TestCase(null)]
        public void WHEN_CultureInfo_is_Empty_SHOULD_throw_ArgumentException(CultureInfo cultureInfo)
        {
            //Arrange
            var membershipViewService = _container.CreateInstance<MembershipViewService>();

            var param = new GetCustomerChangePasswordViewModelParam
            {
                CultureInfo = cultureInfo,
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => membershipViewService.GetChangePasswordViewModelAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("CultureInfo");
        }

        [Test]
        public void WHEN_Customer_is_Null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var membershipViewService = _container.CreateInstance<MembershipViewService>();

            var param = new GetCustomerChangePasswordViewModelParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Scope = GetRandom.String(32),
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => membershipViewService.GetChangePasswordViewModelAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("Customer");
        }
    }
}
