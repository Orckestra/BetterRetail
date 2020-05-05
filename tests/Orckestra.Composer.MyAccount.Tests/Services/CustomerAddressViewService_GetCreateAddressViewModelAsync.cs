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
using Orckestra.Composer.MyAccount.Tests.Mock;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.ForTests;
using Orckestra.ForTests.Mock;

namespace Orckestra.Composer.MyAccount.Tests.Services
{
    class CustomerAddressViewServiceGetCreateAddressViewModelAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(MockMyAccountUrlProviderFactory.Create());
            _container.Use(MockViewModelMapperFactory.Create(typeof(EditAddressViewModel).Assembly));
        }

        [Test]
        public async Task WHEN_passing_valid_arguments_SHOULD_create_viewmodel()
        {
            //Arrange
            var customer = MockCustomerFactory.CreateRandom();

            var mockedCustomerRepository = new Mock<ICustomerRepository>();
            mockedCustomerRepository.Setup(r => r.GetCustomerByIdAsync(It.IsAny<GetCustomerByIdParam>()))
                .ReturnsAsync(customer);
            _container.Use(mockedCustomerRepository);

            var customerViewService = _container.CreateInstance<CustomerAddressViewService>();

            //Act
            var param = new GetCreateAddressViewModelAsyncParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CountryCode = GetRandom.String(2),
            };

            var viewModel = await customerViewService.GetCreateAddressViewModelAsync(param);

            //Assert
            viewModel.Should().NotBeNull("This view model should never be null");
            viewModel.CountryCode.ShouldBeEquivalentTo(param.CountryCode);
            viewModel.FirstName.ShouldBeEquivalentTo(customer.FirstName);
            viewModel.LastName.ShouldBeEquivalentTo(customer.LastName);
            viewModel.PhoneNumber.ShouldBeEquivalentTo(customer.PhoneNumber);
        }

        [Test]
        public void WHEN_Param_is_NULL_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => customerAddressViewService.GetCreateAddressViewModelAsync(null));

            //Assert
            ex.Message.Should().ContainEquivalentOf("param");
        }

        [Test]
        [TestCase(null)]
        public void WHEN_CultureInfo_is_Empty_SHOULD_throw_ArgumentException(CultureInfo cultureInfo)
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new GetCreateAddressViewModelAsyncParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = cultureInfo,
                CountryCode = GetRandom.String(2),
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.GetCreateAddressViewModelAsync(param));

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
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new GetCreateAddressViewModelAsyncParam
            {
                Scope = scope,
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CountryCode = GetRandom.String(2),
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.GetCreateAddressViewModelAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("scope");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void WHEN_CountryCode_is_Empty_SHOULD_throw_ArgumentException(string countryCode)
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new GetCreateAddressViewModelAsyncParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CountryCode = countryCode,
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.GetCreateAddressViewModelAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("CountryCode");
        }

        [Test]
        public void WHEN_CustomerId_is_Null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new GetCreateAddressViewModelAsyncParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CountryCode = GetRandom.String(2),
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.GetCreateAddressViewModelAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("CustomerId");
        }
    }
}
