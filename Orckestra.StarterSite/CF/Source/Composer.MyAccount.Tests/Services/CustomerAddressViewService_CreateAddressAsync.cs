using System;
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
using Orckestra.ForTests;
using Orckestra.ForTests.Mock;
using Orckestra.Overture.ServiceModel;

namespace Orckestra.Composer.MyAccount.Tests.Services
{
    class CustomerAddressViewServiceCreateAddressAsync
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

            var mockedCustomerAddressRepository = new Mock<ICustomerAddressRepository>();
            mockedCustomerAddressRepository.Setup(r => r.CreateAddressAsync(It.IsAny<Guid>(), It.IsAny<Address>(),
                It.IsAny<string>()))
                .ReturnsAsync(address);

            _container.Use(mockedCustomerAddressRepository);

            var customerViewService = _container.CreateInstance<CustomerAddressViewService>();

            //Act
            var viewModel = await customerViewService.CreateAddressAsync(new CreateAddressParam
            {
                ReturnUrl = GetRandom.String(32),
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                EditAddress = new EditAddressRequest()
            });

            //Assert
            viewModel.Should().NotBeNull("This view model should never be null");
            viewModel.FirstName.Should().Be(address.FirstName);
        }

        [Test]
        public void WHEN_Param_is_NULL_SHOULD_throw_ArgumentNullException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();

            //Act
            var ex = Assert.ThrowsAsync<ArgumentNullException>(() => customerAddressViewService.CreateAddressAsync(null));

            //Assert
            ex.Message.Should().ContainEquivalentOf("param");
        }

        [Test]
        [TestCase(null)]
        public void WHEN_CultureInfo_is_Empty_SHOULD_throw_ArgumentException(CultureInfo cultureInfo)
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new CreateAddressParam
            {
                ReturnUrl = GetRandom.String(32),
                Scope = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                EditAddress = new EditAddressRequest(),
                CultureInfo = cultureInfo
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.CreateAddressAsync(param));

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
            var param = new CreateAddressParam
            {
                ReturnUrl = GetRandom.String(32),
                CustomerId = Guid.NewGuid(),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                EditAddress = new EditAddressRequest(),
                Scope = scope
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.CreateAddressAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("scope");
        }

        [Test]
        public void WHEN_CustomerId_is_Null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new CreateAddressParam
            {
                ReturnUrl = GetRandom.String(32),
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                EditAddress = new EditAddressRequest()
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.CreateAddressAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("Customer");
        }

        [Test]
        public void WHEN_EditAddress_is_Null_SHOULD_throw_ArgumentException()
        {
            //Arrange
            var customerAddressViewService = _container.CreateInstance<CustomerAddressViewService>();
            var param = new CreateAddressParam
            {
                ReturnUrl = GetRandom.String(32),
                Scope = GetRandom.String(32),
                CultureInfo = TestingExtensions.GetRandomCulture(),
                CustomerId = Guid.NewGuid(),
            };

            //Act
            var ex = Assert.ThrowsAsync<ArgumentException>(() => customerAddressViewService.CreateAddressAsync(param));

            //Assert
            ex.Message.Should().ContainEquivalentOf("EditAddress");
        }
        
    }
}
