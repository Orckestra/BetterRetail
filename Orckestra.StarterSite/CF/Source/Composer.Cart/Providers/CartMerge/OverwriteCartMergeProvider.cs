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

namespace Orckestra.Composer.Cart.Providers.CartMerge
{
    public class OverwriteCartMergeProvider : ICartMergeProvider
    {
        protected virtual ICartRepository CartRepository { get; private set; }
        protected virtual IFixCartService FixCartService { get; private set; }

        public OverwriteCartMergeProvider(ICartRepository cartRepository, IFixCartService fixCartService)
        {
            if (cartRepository == null) { throw new ArgumentNullException("cartRepository"); }
            if (fixCartService == null) { throw new ArgumentNullException("fixCartService"); }

            CartRepository = cartRepository;
            FixCartService = fixCartService;
        }

        /// <summary>
        /// Overwrites the lineitems of a logger customer's cart with the lineitems of a guest customer's cart if it is not empty.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task MergeCartAsync(CartMergeParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.GuestCustomerId == Guid.Empty) { throw new ArgumentException("param.GuestCustomerId"); }
            if (param.LoggedCustomerId == Guid.Empty) { throw new ArgumentException("param.LoggedCustomerId"); }
            if (string.IsNullOrEmpty(param.Scope)) { throw new ArgumentException("param.Scope"); }

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

            Task.WaitAll(getLoggedCustomerTask, getGuestCustomerTask);

            var loggedCustomerCart = getLoggedCustomerTask.Result;
            var guestCustomerCart = getGuestCustomerTask.Result;

            var guestCustomerLineItems = guestCustomerCart.GetLineItems();

            if (!guestCustomerLineItems.Any())
            {
                return;
            }

            loggedCustomerCart.Shipments.First().LineItems = guestCustomerLineItems;
            loggedCustomerCart.Coupons = guestCustomerCart.Coupons;

            var cart = await CartRepository.UpdateCartAsync(UpdateCartParamFactory.Build(loggedCustomerCart)).ConfigureAwait(false);

            await FixCartService.FixCartAsync(new FixCartParam
            {
                Cart = cart
            });
        }
    }
}
