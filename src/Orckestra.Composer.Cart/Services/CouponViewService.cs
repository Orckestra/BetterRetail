using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Services;
using Orckestra.Overture.ServiceModel.Marketing;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Services
{
    public class CouponViewService : ICouponViewService
    {
        protected ICartRepository CartRepository { get; private set; }
        protected ICartViewModelFactory CartViewModelFactory { get; private set; }
        protected ILocalizationProvider LocalizationProvider { get; private set; }
        protected IImageService ImageService { get; private set; }

        public CouponViewService(ICartRepository cartRepository, 
            ICartViewModelFactory cartViewModelFactory, 
            ILocalizationProvider localizationProvider,
            IImageService imageService)
        {
            CartRepository = cartRepository;
            CartViewModelFactory = cartViewModelFactory;
            LocalizationProvider = localizationProvider;
            ImageService = imageService;
        }

        /// <summary>
        /// Gets all invalid coupon codes.
        /// </summary>
        /// <param name="coupons"></param>
        /// <returns></returns>
        public virtual IEnumerable<string> GetInvalidCouponsCode(IEnumerable<Coupon> coupons)
        {
            if(coupons == null) { return Enumerable.Empty<string>(); }

            return from c in coupons where c.CouponState != CouponState.Ok select c.CouponCode;
        }

        /// <summary>
        /// Adds a coupon to the Cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The lightweight CartViewModel</returns>
        public virtual async Task<CartViewModel> AddCouponAsync(CouponParam param)
        {
            var cart = await CartRepository.AddCouponAsync(param).ConfigureAwait(false);

            await CartRepository.RemoveCouponsAsync(new RemoveCouponsParam
            {
                CartName = param.CartName,
                CouponCodes = GetInvalidCouponsCode(cart.Coupons).ToList(),
                CustomerId = param.CustomerId,
                Scope = param.Scope
            }).ConfigureAwait(false);

            var vmParam = new CreateCartViewModelParam
            {
                Cart = cart,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = true,
                BaseUrl = param.BaseUrl
            };

            var viewModel = await CreateCartViewModelAsync(vmParam).ConfigureAwait(false);

            AddSuccessMessageIfRequired(param, viewModel, param.CultureInfo);

            return viewModel;
        }

        protected virtual void AddSuccessMessageIfRequired(CouponParam param, CartViewModel viewModel, CultureInfo cultureInfo)
        {
            if (viewModel.Coupons.ApplicableCoupons.Any(
                c => string.Equals(c.CouponCode, param.CouponCode, StringComparison.InvariantCultureIgnoreCase)))
            {
                var templateMessage = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
                {
                    Category = "ShoppingCart",
                    Key = "F_PromoCodeSucces",
                    CultureInfo = cultureInfo
                });

                viewModel.Coupons.Messages.Add(new CartMessageViewModel
                {
                    Message = string.Format(templateMessage, param.CouponCode),
                    Level = CartMessageLevels.Success
                });
            }
        }

        /// <summary>
        /// Removes a coupon from the Cart.
        /// </summary>
        /// <param name="param"></param>
        /// <returns>The lightweight CartViewModel</returns>
        public virtual async Task<CartViewModel> RemoveCouponAsync(CouponParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            await CartRepository.RemoveCouponsAsync(new RemoveCouponsParam
            {
                CartName = param.CartName,
                CouponCodes = param.CouponCode == null
                    ? null
                    : new List<string>
                    {
                        param.CouponCode
                    },
                CustomerId = param.CustomerId,
                Scope = param.Scope
            }).ConfigureAwait(false);

            var cart = await CartRepository.GetCartAsync(new GetCartParam
            {
                CartName = param.CartName,
                CultureInfo = param.CultureInfo,
                CustomerId = param.CustomerId,
                Scope = param.Scope,
                ExecuteWorkflow = true
            }).ConfigureAwait(false);

            var vmParam = new CreateCartViewModelParam
            {
                Cart = cart,
                CultureInfo = param.CultureInfo,
                IncludeInvalidCouponsMessages = true,
                BaseUrl = param.BaseUrl
            };

            var viewModel = await CreateCartViewModelAsync(vmParam).ConfigureAwait(false);
            return viewModel;
        }

        protected virtual async Task<CartViewModel> CreateCartViewModelAsync(CreateCartViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.Cart == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.Cart)), nameof(param)); }
            if (param.BaseUrl == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.BaseUrl)), nameof(param)); }

            param.ProductImageInfo = new ProductImageInfo
            {
                ImageUrls = await ImageService.GetImageUrlsAsync(param.Cart.GetLineItems()).ConfigureAwait(false)
            };

            var vm = CartViewModelFactory.CreateCartViewModel(param);
            return vm;
        }
    }
}