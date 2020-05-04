using System;
using System.Globalization;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.MyAccount.Tests.Mock;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Composer.Tests.Mock;
using Orckestra.ForTests;
using Orckestra.ForTests.Mock;
using System.Threading.Tasks;

namespace Orckestra.Composer.MyAccount.Tests.Services
{
    // ReSharper disable once InconsistentNaming
    class CustomerViewService_GetAccountHeaderViewModelAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(MockMyAccountUrlProviderFactory.Create());
            _container.Use(MockViewModelMapperFactory.Create(typeof(AccountHeaderViewModel).Assembly));
        }

        [Test]
        public async Task WHEN_passing_valid_arguments_SHOULD_create_viewmodel()
        {
            //Arrange
            var customer = MockCustomerFactory.CreateRandom();

            var mockedCustomerRepository = new Mock<ICustomerRepository>();
            mockedCustomerRepository.Setup(r => r.GetCustomerByIdAsync(It.Is<GetCustomerByIdParam>(p => p.CustomerId == customer.Id)))
                .ReturnsAsync(customer);
            _container.Use(mockedCustomerRepository);

            var param = new GetAccountHeaderViewModelParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = customer.Id,
                Scope = GetRandom.String(32)
            };

            var customerViewService = _container.CreateInstance<CustomerViewService>();

            //Act
            var viewModel = await customerViewService.GetAccountHeaderViewModelAsync(param);

            //Assert
            viewModel.Should().NotBeNull("This view model should never be null");
            viewModel.FirstName.Should().Be(customer.FirstName);
            viewModel.LastName.Should().Be(customer.LastName);
        }

        [Test]
        public void WHEN_Param_is_NULL_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var customerViewService = _container.CreateInstance<CustomerViewService>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => customerViewService.GetAccountHeaderViewModelAsync(null));

            //Assert
            ex.Message.Should().ContainEquivalentOf("param");
        }

        [Test]
        [TestCase(null)]
        public void WHEN_CultureInfo_is_Empty_SHOULD_throw_ArgumentException(CultureInfo cultureInfo)
        {
            //Arrange
            var customerViewService = _container.CreateInstance<CustomerViewService>();
            var param = new GetAccountHeaderViewModelParam
            {
                CultureInfo = cultureInfo,
                CustomerId = Guid.NewGuid(),
                Scope = GetRandom.String(32)
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerViewService.GetAccountHeaderViewModelAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("CultureInfo");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void WHEN_Scope_is_Empty_SHOULD_throw_ArgumentException(string scope)
        {
            //Arrange
            var customerViewService = _container.CreateInstance<CustomerViewService>();
            var param = new GetAccountHeaderViewModelParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.NewGuid(),
                Scope = scope
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerViewService.GetAccountHeaderViewModelAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("scope");
        }

        [Test]
        public void WHEN_CustomerId_is_Null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var customerViewService = _container.CreateInstance<CustomerViewService>();
            var param = new GetAccountHeaderViewModelParam
            {
                CultureInfo = TestingExtensions.GetRandomCulture(),
                Scope = GetRandom.String(32)
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerViewService.GetAccountHeaderViewModelAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("Customer");
        }
    }
}
