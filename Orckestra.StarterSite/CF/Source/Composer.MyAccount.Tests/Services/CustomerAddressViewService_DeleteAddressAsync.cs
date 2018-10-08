using System;
using System.Collections.Generic;
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
using Orckestra.ForTests.Mock;
using Orckestra.Overture.ServiceModel.Customers;

namespace Orckestra.Composer.MyAccount.Tests.Services
{
    class CustomerAddressViewServiceDeleteAddressAsync
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
            var addressId = Guid.NewGuid();
            var customer = new Customer
            {
                AddressIds = new List<Guid> { addressId }
            };

            var mockedCustomerRepository = new Mock<ICustomerRepository>();
            mockedCustomerRepository.Setup(r => r.GetCustomerByIdAsync(It.IsAny<GetCustomerByIdParam>())).ReturnsAsync(customer);
            _container.Use(mockedCustomerRepository);

            var customerViewService = _container.CreateInstance<CustomerAddressViewService>();

            //Act
            var viewModel = await customerViewService.DeleteAddressAsync(new DeleteAddressParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                AddressId = addressId
            });

            //Assert
            viewModel.Should().NotBeNull();
        }

        [Test]
        public async Task WHEN_passing_valid_arguments_SHOULD_delete_address()
        {
            //Arrange
            var addressId = Guid.NewGuid();
            var customer = new Customer
            {
                AddressIds = new List<Guid> { addressId }
            };

            var mockedCustomerRepository = new Mock<ICustomerRepository>();
            mockedCustomerRepository.Setup(r => r.GetCustomerByIdAsync(It.IsAny<GetCustomerByIdParam>()))
                .ReturnsAsync(customer);
            _container.Use(mockedCustomerRepository);

            var customerViewService = _container.CreateInstance<CustomerAddressViewService>();

            //Act
            await customerViewService.DeleteAddressAsync(new DeleteAddressParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                AddressId = addressId
            });

            //Assert
            _container.GetMock<ICustomerAddressRepository>().Verify(repository => repository.DeleteAddressAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public async Task WHEN_address_does_not_belong_to_the_customer_SHOULD_return_null()
        {
            //Arrange
            var addressId = Guid.NewGuid();
            var customer = new Customer
            {
                AddressIds = new List<Guid>()
            };

            var mockedCustomerRepository = new Mock<ICustomerRepository>();
            mockedCustomerRepository.Setup(r => r.GetCustomerByIdAsync(It.IsAny<GetCustomerByIdParam>()))
                .ReturnsAsync(customer);
            _container.Use(mockedCustomerRepository);

            var customerViewService = _container.CreateInstance<CustomerAddressViewService>();

            //Act
            var viewModel = await customerViewService.DeleteAddressAsync(new DeleteAddressParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                AddressId = addressId
            });

            //Assert
            viewModel.Should().BeNull();
        }

        [Test]
        public void WHEN_Param_is_NULL_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();

            //Act
            var ex = Assert.Throws<ArgumentNullException>(async () => await customerAddressViewService.DeleteAddressAsync(null));

            //Assert
            ex.Message.Should().ContainEquivalentOf("param");
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void WHEN_Scope_is_Empty_SHOULD_throw_ArgumentException(string scope)
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new DeleteAddressParam
            {
                Scope = scope,
                CustomerId = Guid.NewGuid(),
                AddressId = Guid.NewGuid()
            };

            //Act
            var ex = Assert.Throws<ArgumentException>(async () => await customerAddressViewService.DeleteAddressAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("Scope");
        }

        [Test]
        public void WHEN_CustomerId_is_Null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new DeleteAddressParam
            {
                Scope = GetRandom.String(32),
                AddressId = Guid.NewGuid()
            };

            //Act
            var ex = Assert.Throws<ArgumentException>(async () => await customerAddressViewService.DeleteAddressAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("CustomerId");
        }

        [Test]
        public void WHEN_AddressId_is_Null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new DeleteAddressParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
            };

            //Act
            var ex = Assert.Throws<ArgumentException>(async () => await customerAddressViewService.DeleteAddressAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("AddressId");
        }
    }
}
