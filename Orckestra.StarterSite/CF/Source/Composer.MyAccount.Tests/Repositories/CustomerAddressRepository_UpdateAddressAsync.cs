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
using Orckestra.Overture.ServiceModel.Requests;

namespace Orckestra.Composer.MyAccount.Tests.Repositories
{
    public class CustomerAddressRepositoryUpdateAddressAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<IOvertureClient>(MockBehavior.Strict));
        }

        [Test]
        public async Task WHEN_valid_request_SHOULD_succeed()
        {
            //Arrange
            var customerId = GetRandom.Guid();
            var address = MockAddressFactory.CreateRandom();

            var customerRepository = _container.CreateInstance<CustomerAddressRepository>();

            _container.GetMock<IOvertureClient>()
                      .Setup(r => r.SendAsync(It.IsAny<UpdateAddressRequest>()))
                      .ReturnsAsync(address);

            //Act
            var result = await customerRepository.UpdateAddressAsync(customerId, address).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
            result.Line1.ShouldBeEquivalentTo(address.Line1);
        }

        [Test]
        public async Task WHEN_update_address_SHOULD_replace_cache_entry()
        {
            //Arrange
            var customerId = GetRandom.Guid();
            var address = MockAddressFactory.CreateRandom();

            var customerRepository = _container.CreateInstance<CustomerAddressRepository>();

            _container.GetMock<IOvertureClient>()
                      .Setup(r => r.SendAsync(It.IsAny<UpdateAddressRequest>()))
                      .ReturnsAsync(address);

            //Act
            await customerRepository.UpdateAddressAsync(customerId, address).ConfigureAwait(false);

            //Assert
            //3.8 upgrade
            _container
                .GetMock<ICacheProvider>()
                .Verify(provider => provider.SetAsync(It.IsAny<CacheKey>(), It.IsAny<Address>(), It.IsAny<CacheKey>()), Times.Once);
        }
    }
}
