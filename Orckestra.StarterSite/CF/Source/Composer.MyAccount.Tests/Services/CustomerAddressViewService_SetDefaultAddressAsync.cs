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
using Orckestra.Composer.Repositories;
using Orckestra.ForTests.Mock;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Customers;

namespace Orckestra.Composer.MyAccount.Tests.Services
{
    class CustomerAddressViewServiceSetDefaultAddressAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(MockViewModelMapperFactory.Create(typeof(SetDefaultAddressStatusViewModel).Assembly));
        }

        [Test]
        public async Task WHEN_passing_valid_arguments_SHOULD_create_viewmodel()
        {
            //Arrange
            var address = MockAddressFactory.CreateRandom();
            var customer = new Customer
            {
                AddressIds = new List<Guid>
                {
                    address.Id
                }
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
            var viewModel = await customerViewService.SetDefaultAddressAsync(new SetDefaultAddressParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                AddressId = address.Id
            });

            //Assert
            viewModel.Should().NotBeNull();
        }

        [Test]
        public async Task WHEN_address_does_not_belong_to_the_customer_SHOULD_return_null()
        {
            //Arrange
            var address = MockAddressFactory.CreateRandom();
            var customer = new Customer
            {
                AddressIds = new List<Guid>()
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
            var viewModel = await customerViewService.SetDefaultAddressAsync(new SetDefaultAddressParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                AddressId = address.Id
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
            var ex = Assert.Throws<ArgumentNullException>(async () => await customerAddressViewService.SetDefaultAddressAsync(null));

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
            var param = new SetDefaultAddressParam
            {
                Scope = scope,
                CustomerId = Guid.NewGuid(),
                AddressId = Guid.NewGuid()
            };

            //Act
            var ex = Assert.Throws<ArgumentException>(async () => await customerAddressViewService.SetDefaultAddressAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("Scope");
        }

        [Test]
        public void WHEN_CustomerId_is_Null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new SetDefaultAddressParam
            {
                Scope = GetRandom.String(32),
                AddressId = Guid.NewGuid()
            };

            //Act
            var ex = Assert.Throws<ArgumentException>(async () => await customerAddressViewService.SetDefaultAddressAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("CustomerId");
        }

        [Test]
        public void WHEN_AddressId_is_Null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new SetDefaultAddressParam
            {
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
            };

            //Act
            var ex = Assert.Throws<ArgumentException>(async () => await customerAddressViewService.SetDefaultAddressAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("AddressId");
        }
    }
}
