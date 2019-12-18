using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.MyAccount.Repositories;
using Orckestra.Composer.MyAccount.Tests.Mock;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Requests.Customers;

namespace Orckestra.Composer.MyAccount.Tests.Repositories
{
    // ReSharper disable once InconsistentNaming
    internal class CustomerRepository_GetCustomerByTicketAsync
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
            var expectedTicket = GetRandom.String(1024);
            var expectedCustomer = MockCustomerFactory.CreateRandom();
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            _container.GetMock<IOvertureClient>()
                .Setup(r => r.SendAsync(It.Is<GetCustomerFromPasswordTicketRequest>(param => param.Ticket == expectedTicket)))
                .ReturnsAsync(expectedCustomer);

            //Act
            var result = await customerRepository.GetCustomerByTicketAsync(expectedTicket);

            //Assert
            result.Id.Should().Be(expectedCustomer.Id);
        }
    }
}
