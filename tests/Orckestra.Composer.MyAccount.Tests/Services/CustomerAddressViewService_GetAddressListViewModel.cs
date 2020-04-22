using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
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
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.MyAccount.Tests.Services
{
    class CustomerAddressViewServiceGetAddressListViewModel
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
            var address = MockAddressFactory.CreateRandom();
            var customer = MockCustomerFactory.CreateRandom();
            customer.Addresses = new List<Address>
            {
                address
            };

            var mockedCustomerAddressRepository = new Mock<ICustomerAddressRepository>();
            mockedCustomerAddressRepository.Setup(r => r.GetCustomerAddressesAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new List<Address> { address });
            _container.Use(mockedCustomerAddressRepository);

            var mockedCustomerRepository = new Mock<ICustomerRepository>();
            mockedCustomerRepository.Setup(r => r.GetCustomerByIdAsync(It.IsAny<GetCustomerByIdParam>()))
                .ReturnsAsync(customer);
            _container.Use(mockedCustomerRepository);

            var customerViewService = _container.CreateInstance<CustomerAddressViewService>();

            //Act
            var param = new GetAddressListViewModelParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                AddAddressUrl = GetRandom.String(32),
                EditAddressBaseUrl = GetRandom.String(32),
                CountryCode = GetRandom.String(32)
            };

            var viewModel = await customerViewService.GetAddressListViewModelAsync(param);

            //Assert
            viewModel.Should().NotBeNull("This view model should never be null");
            viewModel.AddAddressUrl.ShouldBeEquivalentTo(param.AddAddressUrl);
            viewModel.Addresses.First().AddressName.ShouldBeEquivalentTo(address.AddressName);
        }

        [Test]
        public void WHEN_Param_is_NULL_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => customerAddressViewService.GetAddressListViewModelAsync(null));

            //Assert
            ex.Message.Should().ContainEquivalentOf("param");
        }

        [Test]
        [TestCase(null)]
        public void WHEN_CultureInfo_is_Empty_SHOULD_throw_ArgumentException(CultureInfo cultureInfo)
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new GetAddressListViewModelParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                AddAddressUrl = GetRandom.String(32),
                EditAddressBaseUrl = GetRandom.String(32),
                CultureInfo = cultureInfo,
                CountryCode = GetRandom.String(32)
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.GetAddressListViewModelAsync(param));

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
            var param = new GetAddressListViewModelParam
            {
                Scope = scope,
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                AddAddressUrl = GetRandom.String(32),
                EditAddressBaseUrl = GetRandom.String(32),
                CountryCode = GetRandom.String(32)
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.GetAddressListViewModelAsync(param));

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
            var param = new GetAddressListViewModelParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                AddAddressUrl = GetRandom.String(32),
                EditAddressBaseUrl = GetRandom.String(32),
                CountryCode = countryCode
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.GetAddressListViewModelAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("CountryCode");
        }

        [Test]
        public void WHEN_CustomerId_is_Null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new GetAddressListViewModelParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                AddAddressUrl = GetRandom.String(32),
                EditAddressBaseUrl = GetRandom.String(32),
                CountryCode = GetRandom.String(32)
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.GetAddressListViewModelAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("CustomerId");
        }

    }
}
