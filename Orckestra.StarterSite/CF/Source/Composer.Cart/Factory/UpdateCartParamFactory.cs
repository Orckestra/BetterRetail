using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Parameters;

namespace Orckestra.Composer.Cart.Factory
{
    public static class UpdateCartParamFactory
    {
        public static UpdateCartParam Build(Overture.ServiceModel.Orders.Cart cart)
        {
            return new UpdateCartParam
            {
                CultureInfo = cart.CultureName != null ? new CultureInfo(cart.CultureName) : null,
                CustomerId = cart.CustomerId,
                Scope = cart.ScopeId,
                CartName = cart.Name,
                BillingCurrency = cart.BillingCurrency,
                CartType = cart.CartType,
                Coupons = cart.Coupons,
                Customer = cart.Customer,
                OrderLocation = cart.OrderLocation,
                PropertyBag = cart.PropertyBag,
                Shipments = cart.Shipments,
                Status = cart.Status,
                Payments = cart.Payments
            };
        }
    }
}
