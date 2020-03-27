using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Overture.ServiceModel.Marketing;
using Orckestra.Overture.ServiceModel.Orders;

namespace Orckestra.Composer.Cart.Providers.CartMerge
{
    public class CartMergeProvider : ICartMergeProvider
    {
        protected virtual ICartRepository CartRepository { get; private set; }
        protected virtual IFixCartService FixCartService { get; private set; }

        public CartMergeProvider(ICartRepository cartRepository, IFixCartService fixCartService)
        {
            CartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            FixCartService = fixCartService ?? throw new ArgumentNullException(nameof(fixCartService));
        }

        /// <summary>
        /// Merges the lineitems of a guest customer's cart and a logged customer's cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task MergeCartAsync(CartMergeParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.GuestCustomerId == Guid.Empty) { throw new ArgumentException("param.GuestCustomerId"); }
            if (param.LoggedCustomerId == Guid.Empty) { throw new ArgumentException("param.LoggedCustomerId"); }
            if (string.IsNullOrEmpty(param.Scope)) { throw new ArgumentException("param.Scope"); }

            if (param.GuestCustomerId == param.LoggedCustomerId) { return; }

            var getLoggedCustomerCartTask = CartRepository.GetCartAsync(new GetCartParam
            {
                CustomerId = param.LoggedCustomerId,
                CartName = CartConfiguration.ShoppingCartName,
                Scope = param.Scope,
                CultureInfo = CultureInfo.InvariantCulture
            });

            var getGuestCustomerCartTask = CartRepository.GetCartAsync(new GetCartParam
            {
                CustomerId = param.GuestCustomerId,
                CartName = CartConfiguration.ShoppingCartName,
                Scope = param.Scope,
                CultureInfo = CultureInfo.InvariantCulture
            });

            var result = await Task.WhenAll(getLoggedCustomerCartTask, getGuestCustomerCartTask).ConfigureAwait(false);

            var loggedCustomerCart = result[0];
            var guestCustomerCart = result[1];

            var guestCustomerLineItems = guestCustomerCart.GetLineItems();
            var loggedCustomerLineItems = loggedCustomerCart.GetLineItems();

            if (!guestCustomerLineItems.Any())
            {
                return;
            }

            loggedCustomerCart.Shipments.First().LineItems = MergeLineItems(guestCustomerLineItems, loggedCustomerLineItems);
            loggedCustomerCart.Coupons = MergeCoupons(guestCustomerCart.Coupons, loggedCustomerCart.Coupons);

            var cart = await CartRepository.UpdateCartAsync(UpdateCartParamFactory.Build(loggedCustomerCart)).ConfigureAwait(false);

            await FixCartService.FixCartAsync(new FixCartParam
             {
                 Cart = cart
             });
        }

        protected virtual List<Coupon> MergeCoupons(List<Coupon> guestCoupons, List<Coupon> loggedCustomerCoupons)
        {
            if (guestCoupons == null) { return loggedCustomerCoupons; }

            foreach (var guestCoupon in guestCoupons)
            {
                if (loggedCustomerCoupons.All(c => c.CouponCode != guestCoupon.CouponCode))
                {
                    loggedCustomerCoupons.Add(guestCoupon);
                }
            }

            return loggedCustomerCoupons;
        }

        protected virtual List<LineItem> MergeLineItems(List<LineItem> guestLineItems, List<LineItem> loggedLineItems)
        {
            var mergedLineItems = new List<LineItem>();

            foreach (var lineItem in guestLineItems.Concat(loggedLineItems))
            {
                var mergeLineItem = mergedLineItems.FirstOrDefault(x => x.ProductId == lineItem.ProductId && x.VariantId == lineItem.VariantId);

                if (mergeLineItem == null)
                {
                    mergedLineItems.Add(lineItem);
                }
                else
                {
                    mergeLineItem.Quantity += lineItem.Quantity;
                }
            }

            return mergedLineItems;
        }
    }
}
