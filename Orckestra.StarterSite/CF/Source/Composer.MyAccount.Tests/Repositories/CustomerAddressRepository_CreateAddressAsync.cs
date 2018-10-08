using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.Composer.MyAccount.Tests.Mock;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Requests.Customers;

namespace Orckestra.Composer.MyAccount.Tests.Repositories
{
    public class CustomerAddressRepositoryCreateAddressAsync
    {
        private AutoMocker _container;
        private CustomerAddressRepository _customerRepository;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<IOvertureClient>(MockBehavior.Strict));
            _customerRepository = _container.CreateInstance<CustomerAddressRepository>();
        }

        [Test]
        public async Task WHEN_valid_request_SHOULD_succeed()
        {
            //Arrange
            var customerId = GetRandom.Guid();
            var scope = GetRandom.String(32);
            var address = MockAddressFactory.CreateRandom();
            
            _container.GetMock<IOvertureClient>()
                      .Setup(r => r.SendAsync(It.Is<AddAddressToCustomerRequest>(
                          param => param.CustomerId == customerId &&
                          param.ScopeId == scope)))
                      .ReturnsAsync(address);

            //Act
            var result = await _customerRepository.CreateAddressAsync(customerId, address, scope).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
            result.Line1.ShouldBeEquivalentTo(address.Line1);
        }

        [Test]
        public async Task WHEN_create_address_SHOULD_add_cache_entry()
        {
            //Arrange
            var customerId = GetRandom.Guid();
            var scope = GetRandom.String(32);
            var address = MockAddressFactory.CreateRandom();

            _container.GetMock<IOvertureClient>()
                      .Setup(r => r.SendAsync(It.Is<AddAddressToCustomerRequest>(
                          param => param.CustomerId == customerId &&
                          param.ScopeId == scope)))
                      .ReturnsAsync(address);

            //Act
            await _customerRepository.CreateAddressAsync(customerId, address, scope).ConfigureAwait(false);

            //Assert
            //3.8 upgrade
            _container.GetMock<ICacheProvider>().Verify(provider => provider.SetAsync(It.IsAny<CacheKey>(), It.IsAny<Address>(), It.IsAny<CacheKey>()), Times.Once);
        }
    }
}
