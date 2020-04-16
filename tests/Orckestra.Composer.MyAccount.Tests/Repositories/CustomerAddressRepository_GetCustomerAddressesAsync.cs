using System;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Requests.Customers;

namespace Orckestra.Composer.MyAccount.Tests.Repositories
{
    // ReSharper disable once InconsistentNaming
    public class CustomerAddressRepository_GetCustomerAddressesAsync
    {
        readonly AutoMocker _container = new AutoMocker();

        [Test]
        public void WHEN_CustomerId_Paramater_Is_Empty_Guid_SHOULD_Throw_ArgumentException()
        {
            var repo = _container.CreateInstance<CustomerAddressRepository>();

            repo.Awaiting(r => r.GetCustomerAddressesAsync(Guid.Empty, "Canada")).ShouldThrow<ArgumentException>();
        }

        [Test]
        public void WHEN_ScopeId_Is_Null_SHOULD_Throw_ArgumentNullException()
        {
            var repo = _container.CreateInstance<CustomerAddressRepository>();

            repo.Awaiting(r => r.GetCustomerAddressesAsync(Guid.NewGuid(), null)).ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void WHEN_Calling_GetCustomerAddressAsync_SHOULD_Call_OvertureClient_SendAsync()
        {
            var overtureClient = _container.GetMock<IOvertureClient>();
            var repo = _container.CreateInstance<CustomerAddressRepository>();

            repo.GetCustomerAddressesAsync(Guid.NewGuid(), "Burma");

            overtureClient.Verify(o => o.SendAsync(It.IsAny<GetCustomerAddressesRequest>()));
        }
    }
}
