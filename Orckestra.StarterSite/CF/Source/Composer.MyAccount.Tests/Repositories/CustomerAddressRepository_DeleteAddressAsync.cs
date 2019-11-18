using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.ForTests.Mock;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Requests.Customers.Addresses;

namespace Orckestra.Composer.MyAccount.Tests.Repositories
{
    public class CustomerAddressRepositoryDeleteAddressAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

        }

        [Test]
        public void WHEN_valid_request_SHOULD_succeed()
        {
            //Arrange
            var addressId = GetRandom.Guid();

            var customerRepository = _container.CreateInstance<CustomerAddressRepository>();

            _container.GetMock<IOvertureClient>()
                      .Setup(r => r.SendAsync(It.Is<RemoveAddressRequest>(param => param.AddressId == addressId)))
                      .ReturnsTask();
            
            //Act and Assert
            Assert.DoesNotThrowAsync(() => customerRepository.DeleteAddressAsync(addressId));
        }

        [Test]
        public async Task WHEN_delete_address_SHOULD_remove_cache_entry()
        {
            //Arrange
            var addressId = GetRandom.Guid();

            var customerRepository = _container.CreateInstance<CustomerAddressRepository>();

            _container.GetMock<IOvertureClient>()
                      .Setup(r => r.SendAsync(It.Is<RemoveAddressRequest>(param => param.AddressId == addressId)))
                      .ReturnsTask();

            //Act
            await customerRepository.DeleteAddressAsync(addressId);

            //Assert
            _container.GetMock<ICacheProvider>().Verify(provider => provider.Remove(It.IsAny<CacheKey>()), Times.Once);
        }
    }
   
}
