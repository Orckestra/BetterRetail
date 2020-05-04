using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Providers.CartMerge
{
    public class OverwriteCartMergeProvider : ICartMergeProvider
    {
        protected virtual ICartRepository CartRepository { get; private set; }
        protected virtual IFixCartService FixCartService { get; private set; }

        public OverwriteCartMergeProvider(ICartRepository cartRepository, IFixCartService fixCartService)
        {
            CartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            FixCartService = fixCartService ?? throw new ArgumentNullException(nameof(fixCartService));
        }

        /// <summary>
        /// Overwrites the lineitems of a logger customer's cart with the lineitems of a guest customer's cart if it is not empty.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task MergeCartAsync(CartMergeParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.GuestCustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.GuestCustomerId)), nameof(param)); }
            if (param.LoggedCustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.LoggedCustomerId)), nameof(param)); }
            if (string.IsNullOrEmpty(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            if (param.GuestCustomerId == param.LoggedCustomerId) { return; }

            var getLoggedCustomerTask = CartRepository.GetCartAsync(new GetCartParam
            {
                CustomerId = param.LoggedCustomerId,
                CartName = CartConfiguration.ShoppingCartName,
                Scope = param.Scope,
                CultureInfo = CultureInfo.InvariantCulture,
            });

            var getGuestCustomerTask = CartRepository.GetCartAsync(new GetCartParam
            {
                CustomerId = param.GuestCustomerId,
                CartName = CartConfiguration.ShoppingCartName,
                Scope = param.Scope,
                CultureInfo = CultureInfo.InvariantCulture,
            });

            var result = await Task.WhenAll(getLoggedCustomerTask, getGuestCustomerTask).ConfigureAwait(false);

            var loggedCustomerCart = result[0];
            var guestCustomerCart = result[1];

            var guestCustomerLineItems = guestCustomerCart.GetLineItems();

            if (!guestCustomerLineItems.Any()) { return; }

            loggedCustomerCart.Shipments.First().LineItems = guestCustomerLineItems;
            loggedCustomerCart.Coupons = guestCustomerCart.Coupons;

            var cart = await CartRepository.UpdateCartAsync(UpdateCartParamFactory.Build(loggedCustomerCart));

            await FixCartService.FixCartAsync(new FixCartParam
            {
                Cart = cart
            });
        }
    }
}