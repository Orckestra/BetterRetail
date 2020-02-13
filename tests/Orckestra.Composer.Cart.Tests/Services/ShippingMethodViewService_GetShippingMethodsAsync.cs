using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.Tests.Mock;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Tests.Services
{
    [TestFixture]
    public class ShippingMethodViewServiceGetShippingMethodsAsync
    {
        private AutoMocker _container;

        [SetUp]
        public void SetUp()
        {
            _container = new AutoMocker();

            var mapper = ViewModelMapperFactory.CreateFake(typeof(ShippingMethodViewModel).Assembly);

            _container.Use<IViewModelMapper>(mapper);

            var cartViewModelFactory = _container.CreateInstance<CartViewModelFactory>();
            _container.Use<ICartViewModelFactory>(cartViewModelFactory);
        }

        [Test]
        public async Task WHEN_Passing_Valid_Parameters_SHOULD_Succeed()
        {
            //Arrange
            var mockedFulfillmentMethodRepository = new Mock<IFulfillmentMethodRepository>();

            var fulfillmentMethods = new List<FulfillmentMethod>
            {
                new FulfillmentMethod
                {
                    Id = GetRandom.Guid(),
                    Cost = GetRandom.Double(),
                    ExpectedDeliveryDate = DateTime.UtcNow.AddDays(3),
                    DisplayName = new LocalizedString(new Dictionary<string, string>{{"en-US", GetRandom.String(32)}})
                }
            };

            mockedFulfillmentMethodRepository.Setup(
                r => r.GetCalculatedFulfillmentMethods(It.IsAny<GetShippingMethodsParam>()))
                .ReturnsAsync(fulfillmentMethods);
            _container.Use(mockedFulfillmentMethodRepository);
            var service = _container.CreateInstance<ShippingMethodViewService>();

            //Act
            var result = await service.GetShippingMethodsAsync(new GetShippingMethodsParam
            {
                Scope = GetRandom.String(32),
                CultureInfo = new CultureInfo("en-US"),
                CustomerId = GetRandom.Guid(),
                CartName = GetRandom.String(32),
            }).ConfigureAwait(false);

            //Assert
            result.Should().NotBeNull();
            var shippingMethod = result.ShippingMethods.FirstOrDefault();
            shippingMethod.Should().NotBeNull();
            shippingMethod.Name.ShouldBeEquivalentTo(fulfillmentMethods.First().Name);
            shippingMethod.Cost.ShouldBeEquivalentTo(string.Format("{0}", fulfillmentMethods.First().Cost));
            shippingMethod.ExpectedDaysBeforeDelivery.ShouldBeEquivalentTo(3);
            shippingMethod.DisplayName.ShouldBeEquivalentTo(fulfillmentMethods.First().DisplayName.GetLocalizedValue("en-US"));
        }
    }
}
