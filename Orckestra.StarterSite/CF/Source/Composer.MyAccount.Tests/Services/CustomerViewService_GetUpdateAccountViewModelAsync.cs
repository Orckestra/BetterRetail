using System;
using System.Globalization;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.MyAccount.Tests.Mock;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.ForTests;
using Orckestra.ForTests.Mock;

namespace Orckestra.Composer.MyAccount.Tests.Services
{
    // ReSharper disable once InconsistentNaming
    class CustomerViewService_GetUpdateAccountViewModelAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(MockMyAccountUrlProviderFactory.Create());
            _container.Use(MockViewModelMapperFactory.Create(typeof(UpdateAccountViewModel).Assembly));
        }

        [Test]
        public async void WHEN_passing_valid_arguments_SHOULD_create_viewmodel()
        {
            //Arrange
            var customer = MockCustomerFactory.CreateRandom();

            var mockedCustomerRepository = new Mock<ICustomerRepository>();
            mockedCustomerRepository.Setup(r => r.GetCustomerByIdAsync(It.Is<GetCustomerByIdParam>(p => p.CustomerId == customer.Id)))
                .ReturnsAsync(customer);
            _container.Use(mockedCustomerRepository);

            var expectedStatus = TestingExtensions.GetRandomEnum<MyAccountStatus>();
            var param = new GetUpdateAccountViewModelParam
            {
                Status = expectedStatus,
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = customer.Id,
                ReturnUrl = GetRandom.WwwUrl(),
                Scope = GetRandom.String(32)
            };
            var customerViewService = _container.CreateInstance<CustomerViewService>();

            //Act
            var viewModel = await customerViewService.GetUpdateAccountViewModelAsync(param);

            //Assert
            viewModel.Should().NotBeNull("This view model should never be null");
            viewModel.Status.Should().Be(expectedStatus.ToString("G"), "Because we render the status as a string for HBS message switch");
            viewModel.ReturnUrl.Should().Be(param.ReturnUrl);
            viewModel.Email.Should().Be(customer.Email);
            viewModel.FirstName.Should().Be(customer.FirstName);
            viewModel.LastName.Should().Be(customer.LastName);
        }

        [Test]
        public void WHEN_Param_is_NULL_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var customerViewService = _container.CreateInstance<CustomerViewService>();

            //Act
            var ex = Assert.Throws<ArgumentNullException>(async () => await customerViewService.GetUpdateAccountViewModelAsync(null));

            //Assert
            ex.Message.Should().ContainEquivalentOf("param");
        }

        [Test]
        [TestCase(null)]
        public void WHEN_CultureInfo_is_Empty_SHOULD_throw_ArgumentException(CultureInfo cultureInfo)
        {
            //Arrange
            var customerViewService = _container.CreateInstance<CustomerViewService>();
            var param = new GetUpdateAccountViewModelParam
            {
                Status = TestingExtensions.GetRandomEnum<MyAccountStatus>(),
                CultureInfo = cultureInfo,
                CustomerId = Guid.NewGuid(),
                ReturnUrl = GetRandom.WwwUrl(),
                Scope = GetRandom.String(32)
            };

            //Act
            var ex = Assert.Throws<ArgumentException>(async () => await customerViewService.GetUpdateAccountViewModelAsync(param));

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
            var param = new GetUpdateAccountViewModelParam
            {
                Status = TestingExtensions.GetRandomEnum<MyAccountStatus>(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.NewGuid(),
                ReturnUrl = GetRandom.WwwUrl(),
                Scope = scope
            };

            //Act
            var ex = Assert.Throws<ArgumentException>(async () => await customerViewService.GetUpdateAccountViewModelAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("Scope");
        }


        [Test]
        public void WHEN_Customer_is_Null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var customerViewService = _container.CreateInstance<CustomerViewService>();
            var param = new GetUpdateAccountViewModelParam
            {
                Status = TestingExtensions.GetRandomEnum<MyAccountStatus>(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                ReturnUrl = GetRandom.WwwUrl(),
                Scope = GetRandom.String(32)
            };

            //Act
            var ex = Assert.Throws<ArgumentException>(async () => await customerViewService.GetUpdateAccountViewModelAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("Customer");
        }

        [Test]
        [TestCase(null)]
        public async void WHEN_Status_is_NULL_SHOULD_create_view_model_with_empty_status(MyAccountStatus? status)
        {
            //Arrange
            var customer = MockCustomerFactory.CreateRandom();

            var mockedCustomerRepository = new Mock<ICustomerRepository>();
            mockedCustomerRepository.Setup(r => r.GetCustomerByIdAsync(It.Is<GetCustomerByIdParam>(p => p.CustomerId == customer.Id)))
                .ReturnsAsync(customer);
            _container.Use(mockedCustomerRepository);

            var param = new GetUpdateAccountViewModelParam
            {
                Status = status,
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = customer.Id,
                ReturnUrl = GetRandom.WwwUrl(),
                Scope = GetRandom.String(32)
            };
            var customerViewService = _container.CreateInstance<CustomerViewService>();

            //Act
            var viewModel = await customerViewService.GetUpdateAccountViewModelAsync(param);

            //Assert
            viewModel.Status.Should().BeEmpty();
        }
    }
}
