using System;
using System.Globalization;
using System.Linq;
using FizzWare.NBuilder.Generators;
using FluentAssertions;
using NUnit.Framework;
using Orckestra.Composer.Cart.Factory.Order;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Overture.ServiceModel.Queries;
using Orckestra.Overture.ServiceModel.Requests.Orders;

namespace Orckestra.Composer.Cart.Tests.Factory.Order
{
    public class FindOrdersRequestFactoryCreate
    {
        [Test]
        public void WHEN_page_equal_1_SHOULD_return_right_request()
        {
            //Arrange
            var factory = new FindOrdersRequestFactory();

            //Act
            var param = new GetCustomerOrdersParam
            {
                CustomerId = Guid.NewGuid(),
                CultureInfo = new CultureInfo("en-US"),
                Scope = GetRandom.String(32),
                Page = 1,
                OrderTense = OrderTense.CurrentOrders
            };
            var request = factory.Create(param);

            //Assert
            ValidateRequest(request, param, 0, true);
        }

        [Test]
        public void WHEN_page_equal_2_SHOULD_return_right_request()
        {
            //Arrange
            var factory = new FindOrdersRequestFactory();

            //Act
            var param = new GetCustomerOrdersParam
            {
                CustomerId = Guid.NewGuid(),
                CultureInfo = new CultureInfo("en-US"),
                Scope = GetRandom.String(32),
                Page = 2,
                OrderTense = OrderTense.CurrentOrders
            };
            var request = factory.Create(param);

            //Assert
            ValidateRequest(request, param, 10, true);
        }

        [Test]
        public void WHEN_OrderTense_equal_CurrentOrders_SHOULD_return_right_request()
        {
            //Arrange
            var factory = new FindOrdersRequestFactory();

            //Act
            var param = new GetCustomerOrdersParam
            {
                CustomerId = Guid.NewGuid(),
                CultureInfo = new CultureInfo("en-US"),
                Scope = GetRandom.String(32),
                Page = 2,
                OrderTense = OrderTense.CurrentOrders
            };
            var request = factory.Create(param);

            //Assert
            ValidateRequest(request, param, 10, true);
        }

        [Test]
        public void WHEN_OrderTense_equal_PastOrders_SHOULD_return_right_request()
        {
            //Arrange
            var factory = new FindOrdersRequestFactory();

            //Act
            var param = new GetCustomerOrdersParam
            {
                CustomerId = Guid.NewGuid(),
                CultureInfo = new CultureInfo("en-US"),
                Scope = GetRandom.String(32),
                Page = 2,
                OrderTense = OrderTense.PastOrders
            };
            var request = factory.Create(param);

            //Assert
            ValidateRequest(request, param, 10, false);
        }

        private void ValidateRequest(FindOrdersRequest request, GetCustomerOrdersParam param,
            int expectedStartingIndex, bool expectedNot)
        {
            request.Should().NotBeNull();
            request.CustomerId.ShouldBeEquivalentTo(param.CustomerId);
            request.CultureName.ShouldBeEquivalentTo(param.CultureInfo.Name);
            request.ScopeId.ShouldBeEquivalentTo(param.Scope);
            request.Query.Should().NotBeNull();

            var query = request.Query;
            query.StartingIndex.ShouldBeEquivalentTo(expectedStartingIndex);
            query.MaximumItems.ShouldBeEquivalentTo(OrderHistoryConfiguration.MaxItemsPerPage);
            query.Filter.Should().NotBeNull();
            query.Filter.Filters.Should().NotBeNull();
            var filter = query.Filter.Filters.FirstOrDefault();
            filter.Should().NotBeNull();
            filter.Member.ShouldBeEquivalentTo("OrderStatus");
            filter.Operator.ShouldBeEquivalentTo(Operator.In);
            filter.Not.ShouldBeEquivalentTo(expectedNot);
        }

    }
}
