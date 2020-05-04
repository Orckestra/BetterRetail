using System;
using System.Collections.Generic;
using System.Linq;
using Orckestra.Composer.Cart.Parameters.Order;
using Orckestra.Overture.ServiceModel.Queries;
using Orckestra.Overture.ServiceModel.Requests.Orders;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Factory.Order
{
    public class FindOrdersRequestFactory : IFindOrdersRequestFactory
    {
        public virtual FindOrdersRequest Create(GetCustomerOrdersParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            var request = new FindOrdersRequest
            {
                CustomerId = param.CustomerId,
                CultureName = param.CultureInfo.Name,
                ScopeId = param.Scope,
                Query = new Query
                {
                    StartingIndex = (param.Page - 1) * OrderHistoryConfiguration.MaxItemsPerPage,
                    MaximumItems = OrderHistoryConfiguration.MaxItemsPerPage,
                    Sortings = new List<QuerySorting>
                    {
                        new QuerySorting
                        {
                            Direction = SortDirection.Descending,
                            PropertyName = "Created"
                        }
                    }
                },
            };

            request.Query.Filter = new FilterGroup
            {
                Filters = new List<Filter>
                {
                    BuildOrderStatusFilter(param.OrderTense)
                }
            };

            return request;
        }

        /// <summary>
        /// Builds the order status filter.
        /// </summary>
        /// <param name="orderTense">The order tense.</param>
        /// <returns></returns>
        protected virtual Filter BuildOrderStatusFilter(OrderTense orderTense)
        {
            return new Filter
            {
                Value = Enumerable.ToArray(OrderHistoryConfiguration.CompletedOrderStatuses),
                Member = "OrderStatus",
                Operator = Operator.In,
                Not = orderTense.Equals(OrderTense.CurrentOrders)
            };
        }
    }
}
