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
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

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
			if (param == null) { throw new ArgumentNullException(nameof(param)); }
			if (param.GuestCustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.GuestCustomerId)), nameof(param)); }
			if (param.LoggedCustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.LoggedCustomerId)), nameof(param)); }
			if (string.IsNullOrEmpty(param.Scope)) { throw new ArgumentException(GetMessageOfNullEmpty(nameof(param.Scope)), nameof(param)); }
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

			if (!guestCustomerLineItems.Any())
			{
				return;
			}

			var loggedCustomerLineItems = loggedCustomerCart.GetLineItems();

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
			if (guestCoupons == null || !guestCoupons.Any()) { return loggedCustomerCoupons; }

			var hashSet = new HashSet<string>(loggedCustomerCoupons.Select(x => x.CouponCode));

			foreach (var guestCoupon in guestCoupons)
			{
				if (hashSet.Contains(guestCoupon.CouponCode)) { continue; }

				hashSet.Add(guestCoupon.CouponCode);
				loggedCustomerCoupons.Add(guestCoupon);
			}
			return loggedCustomerCoupons;
		}

		protected virtual List<LineItem> MergeLineItems(List<LineItem> guestLineItems, List<LineItem> loggedLineItems)
		{
			var dictionary = new Dictionary<(string, string), LineItem>();

			if (guestLineItems != null)
			{
				foreach (var guestLineItem in guestLineItems)
				{
					if (dictionary.ContainsKey((guestLineItem.ProductId, guestLineItem.VariantId)))
					{
						dictionary[(guestLineItem.ProductId, guestLineItem.VariantId)].Quantity += guestLineItem.Quantity;
					}
					else
					{
						dictionary.Add((guestLineItem.ProductId, guestLineItem.VariantId), guestLineItem);
					}
				}
			}

			if (loggedLineItems != null)
			{
				foreach (var loggedLineItem in loggedLineItems)
				{
					if (dictionary.ContainsKey((loggedLineItem.ProductId, loggedLineItem.VariantId)))
					{
						dictionary[(loggedLineItem.ProductId, loggedLineItem.VariantId)].Quantity += loggedLineItem.Quantity;
					}
					else
					{
						dictionary.Add((loggedLineItem.ProductId, loggedLineItem.VariantId), loggedLineItem);
					}
				}
			}

			return dictionary.Values.ToList();
		}
	}
}