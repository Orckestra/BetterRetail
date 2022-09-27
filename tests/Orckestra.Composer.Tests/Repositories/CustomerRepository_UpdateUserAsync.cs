using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Tests.Mock;
using Orckestra.Overture.ServiceModel.Requests.Customers;

namespace Orckestra.Composer.Tests.Repositories
{
    class CustomerRepositoryUpdateUserAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();
            _container.Use(new Mock<IComposerOvertureClient>(MockBehavior.Strict));
        }

        [Test]
        public async Task WHEN_valid_request_SHOULD_succeed()
        {
            //Arrange
            var expectedCustomer = MockCustomerFactory.CreateRandom();
            var customerRepository = _container.CreateInstance<CustomerRepository>();

            _container.GetMock<IComposerOvertureClient>()
                .Setup(r => r.SendAsync(It.IsAny<UpdateCustomerRequest>()))
                .ReturnsAsync(expectedCustomer);

            //Act
            var result = await customerRepository.UpdateUserAsync(
                new UpdateUserParam
                {
                    Customer = expectedCustomer,
                    Scope = GetRandom.String(32)
                }
            );

            //Assert
            result.Id.Should().Be(expectedCustomer.Id);
        }
    }
}
