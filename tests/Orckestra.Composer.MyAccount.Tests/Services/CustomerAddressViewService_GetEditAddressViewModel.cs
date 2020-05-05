using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Country;
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
    class CustomerAddressViewServiceGetEditAddressViewModel
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(MockMyAccountUrlProviderFactory.Create());
            _container.Use(MockViewModelMapperFactory.Create(typeof(EditAddressViewModel).Assembly));
            _container.Use(CreateCountryServiceMock());
        }

        private static Mock<ICountryService> CreateCountryServiceMock()
        {
            var country = new Country.CountryViewModel
            {
                PhoneRegex = "/*/",
                PostalCodeRegex = "/*/"
            };

            var countryService = new Mock<ICountryService>(MockBehavior.Strict);

            countryService.Setup(c => c.RetrieveCountryAsync(It.IsAny<RetrieveCountryParam>()))
                .ReturnsAsync(country)
                .Verifiable();

            countryService.Setup(c => c.RetrieveRegionDisplayNameAsync(It.IsAny<RetrieveRegionDisplayNameParam>()))
                .ReturnsAsync(GetRandom.String(32))
                .Verifiable();
            return countryService;
        }

        [Test]
        public async Task WHEN_passing_valid_arguments_SHOULD_create_viewmodel()
        {
            //Arrange
            var customer = MockCustomerFactory.CreateRandom();
            var address = MockAddressFactory.CreateRandom();
            customer.AddressIds = new List<Guid>
            {
                address.Id
            };

            var mockedCustomerRepository = new Mock<ICustomerRepository>();
            mockedCustomerRepository.Setup(r => r.GetCustomerByIdAsync(It.IsAny<GetCustomerByIdParam>()))
                .ReturnsAsync(customer);
            _container.Use(mockedCustomerRepository);

            var mockedAddressRepository = new Mock<IAddressRepository>();
            mockedAddressRepository.Setup(r => r.GetAddressByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(address);
            _container.Use(mockedAddressRepository);


            var customerViewService = _container.CreateInstance<CustomerAddressViewService>();

            //Act
            var param = new GetEditAddressViewModelAsyncParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                AddressId = address.Id,
            };

            var viewModel = await customerViewService.GetEditAddressViewModelAsync(param);

            //Assert
            viewModel.Should().NotBeNull();
            viewModel.FirstName.ShouldBeEquivalentTo(address.FirstName);
            viewModel.LastName.ShouldBeEquivalentTo(address.LastName);
            viewModel.PhoneNumber.ShouldBeEquivalentTo(address.PhoneNumber);
            viewModel.PhoneRegex.Should().NotBeNull();
            viewModel.PostalCodeRegex.Should().NotBeNull();
        }

        [Test]
        public async Task WHEN_address_does_not_belong_to_the_customer_SHOULD_return_null()
        {
            //Arrange
            var customer = MockCustomerFactory.CreateRandom();
            var address = MockAddressFactory.CreateRandom();
            customer.AddressIds = new List<Guid>
            {
               //no address
            };

            var mockedCustomerRepository = new Mock<ICustomerRepository>();
            mockedCustomerRepository.Setup(r => r.GetCustomerByIdAsync(It.IsAny<GetCustomerByIdParam>()))
                .ReturnsAsync(customer);
            _container.Use(mockedCustomerRepository);

            var mockedAddressRepository = new Mock<IAddressRepository>();
            mockedAddressRepository.Setup(r => r.GetAddressByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(address);
            _container.Use(mockedAddressRepository);


            var customerViewService = _container.CreateInstance<CustomerAddressViewService>();

            //Act
            var param = new GetEditAddressViewModelAsyncParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                AddressId = address.Id,
            };

            var viewModel = await customerViewService.GetEditAddressViewModelAsync(param);

            //Assert
            viewModel.Should().BeNull();
        }


        [Test]
        public void WHEN_Param_is_NULL_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => customerAddressViewService.GetEditAddressViewModelAsync(null));

            //Assert
            ex.Message.Should().ContainEquivalentOf("param");
        }

        [Test]
        [TestCase(null)]
        public void WHEN_CultureInfo_is_Empty_SHOULD_throw_ArgumentException(CultureInfo cultureInfo)
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new GetEditAddressViewModelAsyncParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = cultureInfo,
                AddressId = Guid.NewGuid(),
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.GetEditAddressViewModelAsync(param));

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
            var param = new GetEditAddressViewModelAsyncParam
            {
                Scope = scope,
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                AddressId = Guid.NewGuid(),
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.GetEditAddressViewModelAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("scope");
        }

        [Test]
        public void WHEN_CustomerId_is_Null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new GetEditAddressViewModelAsyncParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                AddressId = Guid.NewGuid(),
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.GetEditAddressViewModelAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("CustomerId");
        }

        [Test]
        public void WHEN_AddressId_is_Null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new GetEditAddressViewModelAsyncParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.NewGuid(),
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.GetEditAddressViewModelAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("AddressId");
        }
    }
}
