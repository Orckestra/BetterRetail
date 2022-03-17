using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FizzWare.NBuilder.Generators;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Overture.ServiceModel.Orders;
using Orckestra.Overture.ServiceModel.Requests.RecurringOrders;

namespace Orckestra.Composer.Cart.Tests.Mock
{
    public class CartRepositoryUpdateCartMock : ICartRepository
    {
        public ProcessedCart CurrentCart { get; set; }

        public Task<ProcessedCart> GetCartAsync(GetCartParam param)
        {
            return Task.FromResult(CurrentCart);
        }

        public Task<PaymentMethod> SetDefaultCustomerPaymentMethod(SetDefaultCustomerPaymentMethodParam param)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessedCart> UpdateCartAsync(UpdateCartParam param)
        {
            var processedCart = new ProcessedCart
            {
                CultureName = param.CultureInfo.Name,
                CustomerId = param.CustomerId,
                ScopeId = param.Scope,
                Name = param.CartName,
                BillingCurrency = param.BillingCurrency,
                CartType = param.CartType,
                Coupons = param.Coupons,
                Customer = param.Customer,
                OrderLocation = param.OrderLocation,
                PropertyBag = param.PropertyBag,
                Shipments = param.Shipments,
                Status = param.Status,
            };

            return Task.FromResult(processedCart);
        }

        public Task<ProcessedCart> UpdateShipmentAsync(UpdateShipmentParam param)
        {
            throw new NotImplementedException();
        }

        public Task<Order> CompleteCheckoutAsync(CompleteCheckoutParam param)
        {
            var order = new Order
            {
                Cart = new Overture.ServiceModel.Orders.Cart(),
                CustomerId = param.CustomerId.ToString(),
                ScopeId = param.Scope,
                Id = GetRandom.Guid().ToString(),
                CustomerName = GetRandom.String(25),
                OrderNumber = GetRandom.String(5),
                Created = GetRandom.DateTime(),
                CreatedBy = GetRandom.String(25),
                ItemCount = GetRandom.PositiveInt(),
                LastModified = GetRandom.DateTime(),
                LastModifiedBy = GetRandom.String(25),
                OrderStatus = GetRandom.String(10),
                Source = GetRandom.String(10)
            };

            return Task.FromResult(order);
        }

        public Task<ProcessedCart> AddLineItemAsync(AddLineItemParam param)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessedCart> AddPaymentAsync(AddPaymentParam param)
        {
            throw new NotImplementedException();
        }

        public Task<List<CartSummary>> GetCartsByCustomerIdAsync(GetCartsByCustomerIdParam param)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessedCart> RemoveLineItemAsync(RemoveLineItemParam param)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessedCart> RemoveLineItemsAsync(RemoveLineItemsParam param)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessedCart> UpdateLineItemAsync(UpdateLineItemParam param)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessedCart> AddCouponAsync(CouponParam param)
        {
            throw new NotImplementedException();
        }

        public Task RemoveCouponsAsync(RemoveCouponsParam param)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProcessedCart>> GetRecurringCartsAsync(GetRecurringOrderCartsViewModelParam param)
        {
            throw new NotImplementedException();
        }

        public Task<ListOfRecurringOrderLineItems> RescheduleRecurringCartAsync(RescheduleRecurringCartParam param)
        {
            throw new NotImplementedException();
        }

        public Task<HttpWebResponse> RemoveRecurringCartLineItemAsync(RemoveRecurringCartLineItemParam param)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessedCart> AddLineItemsAsync(AddLineItemsParam param) => throw new NotImplementedException();
        public Task<HttpWebResponse> DeleteCartAsync(DeleteCartParam param) => throw new NotImplementedException();
        public Task<List<LineItem>> GetLineItemsAsync(GetLineItemsParam param) => throw new NotImplementedException();

        public Task<Overture.ServiceModel.Orders.Cart> CreateCartPaymentVaultProfile(CreateCartPaymentVaultProfileParam param)
        {
            throw new NotImplementedException();
        }
    }
}
