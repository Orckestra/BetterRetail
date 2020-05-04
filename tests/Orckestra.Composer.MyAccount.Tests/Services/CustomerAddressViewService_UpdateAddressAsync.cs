using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.Composer.MyAccount.Requests;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.MyAccount.Tests.Mock;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Tests.Mock;
using Orckestra.ForTests;
using Orckestra.ForTests.Mock;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.MyAccount.Tests.Services
{
    class CustomerAddressViewServiceUpdateAddressAsync
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
            customer.AddressIds = new List<Guid>
            {
                address.Id
            };

            var mockedCustomerRepository = new Mock<ICustomerRepository>();
            mockedCustomerRepository.Setup(r => r.GetCustomerByIdAsync(It.IsAny<GetCustomerByIdParam>())).ReturnsAsync(customer);
            _container.Use(mockedCustomerRepository);

            var mockedCustomerAddressRepository = new Mock<ICustomerAddressRepository>();
            mockedCustomerAddressRepository.Setup(r => r.UpdateAddressAsync(It.IsAny<Guid>(), It.IsAny<Address>())).ReturnsAsync(address);
            _container.Use(mockedCustomerAddressRepository);

            var mockedAddressRepository = new Mock<IAddressRepository>();
            mockedAddressRepository.Setup(r => r.GetAddressByIdAsync(It.IsAny<Guid>())).ReturnsAsync(address);
            _container.Use(mockedAddressRepository);

            var customerViewService = _container.CreateInstance<CustomerAddressViewService>();

            //Act
            var param = new EditAddressParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                AddressId = address.Id,
                ReturnUrl = GetRandom.String(32),
                EditAddress = new EditAddressRequest()
            };

            var viewModel = await customerViewService.UpdateAddressAsync(param);

            //Assert
            viewModel.Should().NotBeNull();
            viewModel.Status.ShouldBeEquivalentTo(MyAccountStatus.Success.ToString());
        }

        [Test]
        public async Task WHEN_address_does_not_belong_to_the_customer_SHOULD_return_null()
        {
            //Arrange
            var address = MockAddressFactory.CreateRandom();
            var customer = MockCustomerFactory.CreateRandom();
            customer.AddressIds = new List<Guid>
            {
                //no address
            };

            var mockedCustomerRepository = new Mock<ICustomerRepository>();
            mockedCustomerRepository.Setup(r => r.GetCustomerByIdAsync(It.IsAny<GetCustomerByIdParam>())).ReturnsAsync(customer);
            _container.Use(mockedCustomerRepository);

            var mockedCustomerAddressRepository = new Mock<ICustomerAddressRepository>();
            mockedCustomerAddressRepository.Setup(r => r.UpdateAddressAsync(It.IsAny<Guid>(), It.IsAny<Address>())).ReturnsAsync(address);
            _container.Use(mockedCustomerAddressRepository);

            var mockedAddressRepository = new Mock<IAddressRepository>();
            mockedAddressRepository.Setup(r => r.GetAddressByIdAsync(It.IsAny<Guid>())).ReturnsAsync(address);
            _container.Use(mockedAddressRepository);

            var customerViewService = _container.CreateInstance<CustomerAddressViewService>();

            //Act
            var param = new EditAddressParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                AddressId = address.Id,
                ReturnUrl = GetRandom.String(32),
                EditAddress = new EditAddressRequest()
            };

            var viewModel = await customerViewService.UpdateAddressAsync(param);

            //Assert
            viewModel.Should().BeNull();
        }

        [Test]
        public async Task WHEN_address_does_not_exist_SHOULD_return_null()
        {
            //Arrange
            var address = MockAddressFactory.CreateRandom();
            var customer = MockCustomerFactory.CreateRandom();
            customer.AddressIds = new List<Guid>
            {
            };

            var mockedCustomerRepository = new Mock<ICustomerRepository>();
            mockedCustomerRepository.Setup(r => r.GetCustomerByIdAsync(It.IsAny<GetCustomerByIdParam>())).ReturnsAsync(customer);
            _container.Use(mockedCustomerRepository);

            var mockedCustomerAddressRepository = new Mock<ICustomerAddressRepository>();
            mockedCustomerAddressRepository.Setup(r => r.UpdateAddressAsync(It.IsAny<Guid>(), It.IsAny<Address>())).ReturnsAsync(address);
            _container.Use(mockedCustomerAddressRepository);

            var mockedAddressRepository = new Mock<IAddressRepository>();
            mockedAddressRepository.Setup(r => r.GetAddressByIdAsync(It.IsAny<Guid>())).ReturnsAsync(null); //address does not exist
            _container.Use(mockedAddressRepository);

            var customerViewService = _container.CreateInstance<CustomerAddressViewService>();

            //Act
            var param = new EditAddressParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                AddressId = address.Id,
                ReturnUrl = GetRandom.String(32),
                EditAddress = new EditAddressRequest()
            };

            var viewModel = await customerViewService.UpdateAddressAsync(param);

            //Assert
            viewModel.Should().BeNull();
        }

        [Test]
        public void WHEN_Param_is_NULL_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => customerAddressViewService.UpdateAddressAsync(null));

            //Assert
            ex.Message.Should().ContainEquivalentOf("param");
        }

        [Test]
        [TestCase(null)]
        public void WHEN_CultureInfo_is_Empty_SHOULD_throw_ArgumentException(CultureInfo cultureInfo)
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new EditAddressParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = cultureInfo,
                AddressId = Guid.NewGuid(),
                ReturnUrl = GetRandom.String(32),
                EditAddress = new EditAddressRequest()
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.UpdateAddressAsync(param));

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
            var param = new EditAddressParam
            {
                Scope = scope,
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                AddressId = Guid.NewGuid(),
                ReturnUrl = GetRandom.String(32),
                EditAddress = new EditAddressRequest()
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.UpdateAddressAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("scope");
        }

        [Test]
        public void WHEN_CustomerId_is_Null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new EditAddressParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                AddressId = Guid.NewGuid(),
                ReturnUrl = GetRandom.String(32),
                EditAddress = new EditAddressRequest()
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.UpdateAddressAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("CustomerId");
        }

        [Test]
        public void WHEN_AddressId_is_Null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new EditAddressParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                ReturnUrl = GetRandom.String(32),
                EditAddress = new EditAddressRequest()
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.UpdateAddressAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("AddressId");
        }

        [Test]
        public void WHEN_EditAddress_is_Null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new EditAddressParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                AddressId = Guid.NewGuid(),
                ReturnUrl = GetRandom.String(32),
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.UpdateAddressAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("EditAddress");
        }
    }
}
