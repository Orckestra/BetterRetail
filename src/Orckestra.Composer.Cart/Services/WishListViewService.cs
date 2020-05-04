using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Parameters.WishList;
using Orckestra.Composer.Cart.Providers.WishList;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Providers.Dam;
using Orckestra.Composer.Services;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Services
{
    public class WishListViewService: IWishListViewService
    {
        protected IWishListRepository WishListRepository { get; private set; }
        protected ILineItemViewModelFactory LineItemViewModelFactory { get; private set; }
        protected IWishListUrlProvider WishListUrlProvider { get; private set; }

        protected IFixCartService FixCartService { get; private set; }
        protected IImageService ImageService { get; private set; }

        public WishListViewService(
            IWishListRepository wishListRepository,
            ILineItemViewModelFactory lineItemViewModelFactory,
            IWishListUrlProvider wishListUrlProvider,
            IFixCartService fixCartService,
            IImageService imageService)
        {
            WishListRepository = wishListRepository ?? throw new ArgumentNullException(nameof(wishListRepository));
            LineItemViewModelFactory = lineItemViewModelFactory ?? throw new ArgumentNullException(nameof(lineItemViewModelFactory));
            WishListUrlProvider = wishListUrlProvider ?? throw new ArgumentNullException(nameof(wishListUrlProvider));
            FixCartService = fixCartService ?? throw new ArgumentNullException(nameof(fixCartService));
            ImageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        }

        public virtual async Task<WishListSummaryViewModel> AddLineItemAsync(AddLineItemParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.ProductId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.ProductId)), nameof(param)); }
            if (param.Quantity < 1) { throw new ArgumentOutOfRangeException(nameof(param), param.Quantity, GetMessageOfZeroNegative(nameof(param.Quantity))); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }

            var wishList = await WishListRepository.AddLineItemAsync(param).ConfigureAwait(false);

            return CreateSummaryWishListViewModel(new CreateWishListViewModelParam
            {
                WishList = wishList,
                CultureInfo = param.CultureInfo,
                BaseUrl = param.BaseUrl
            });
        }

        public virtual async Task<WishListSummaryViewModel> RemoveLineItemAsync(RemoveLineItemParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.LineItemId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.LineItemId)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.BaseUrl)), nameof(param)); }

            var wishList = await WishListRepository.RemoveLineItemAsync(param).ConfigureAwait(false);

            return CreateSummaryWishListViewModel(new CreateWishListViewModelParam()
            {
                WishList = wishList,
                CultureInfo = param.CultureInfo,
                BaseUrl = param.BaseUrl
            });
        }

        public virtual async Task<WishListViewModel> GetWishListViewModelAsync(GetCartParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }

            var wishList = await WishListRepository.GetWishListAsync(param).ConfigureAwait(false);
            var fixedWishlist = await FixCartService.SetFulfillmentLocationIfRequired(new FixCartParam
            {
                Cart = wishList
            }).ConfigureAwait(false);

            if (wishList == null) { return null; }

            return await CreateWishListViewModelAsync(new CreateWishListViewModelParam()
            {
                WishList = fixedWishlist,
                CultureInfo = param.CultureInfo,
                BaseUrl = param.BaseUrl
            });
        }

        public virtual async Task<WishListSummaryViewModel> GetWishListSummaryViewModelAsync(GetCartParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CartName)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CartName)), nameof(param)); }
            if (param.CustomerId == Guid.Empty) { throw new ArgumentException(GetMessageOfEmpty(nameof(param.CustomerId)), nameof(param)); }

            var wishList = await WishListRepository.GetWishListAsync(param).ConfigureAwait(false);

            if (wishList == null) { return null; }

            return CreateSummaryWishListViewModel(new CreateWishListViewModelParam
            {
                WishList = wishList,
                CultureInfo = param.CultureInfo,
                BaseUrl = param.BaseUrl,
                WebsiteId = param.WebsiteId
            });
        }

        protected virtual async Task<WishListViewModel> CreateWishListViewModelAsync(CreateWishListViewModelParam param)
        {
            var viewModel = new WishListViewModel();
            var lineItems = param.WishList.GetLineItems();

            if (lineItems != null)
            {
                var imageInfo = new ProductImageInfo
                {
                    ImageUrls = await ImageService.GetImageUrlsAsync(lineItems).ConfigureAwait(false),
                };

                viewModel.Items = LineItemViewModelFactory.CreateViewModel(new CreateListOfLineItemDetailViewModelParam {
                    Cart = param.WishList,
                    LineItems = lineItems,
                    CultureInfo = param.CultureInfo,
                    ImageInfo =  imageInfo,
                    BaseUrl = param.BaseUrl
                    }).ToList();

                viewModel.Items.Reverse();
                viewModel.TotalQuantity = lineItems.Count;
            }

            var getUrlParam = new GetWishListUrlParam
            {
                BaseUrl = param.BaseUrl,
                CultureInfo = param.CultureInfo
            };

            viewModel.SignInUrl = WishListUrlProvider.GetSignInUrl(getUrlParam);
            viewModel.ShareUrl = viewModel.TotalQuantity == 0 ? string.Empty: WishListUrlProvider.GetShareUrl(new GetShareWishListUrlParam
            {
                BaseUrl = param.BaseUrl,
                CultureInfo = param.CultureInfo,
                CustomerId = param.WishList.CustomerId,
                Scope = param.WishList.ScopeId,
                WebsiteId = param.WebsiteId
            });

            return viewModel;
        }

        protected virtual WishListSummaryViewModel CreateSummaryWishListViewModel(CreateWishListViewModelParam param)
        {
            var viewModel = new WishListSummaryViewModel { Items = new List<LineItemIdsViewModel>() };
            var lineItems = param.WishList.GetLineItems();

            if (lineItems != null)
            {
                viewModel.TotalQuantity = lineItems.Count;

                foreach (var item in lineItems)
                {
                    viewModel.Items.Add(new LineItemIdsViewModel
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        VariantId = item.VariantId
                    });
                }
            }

            viewModel.SignInUrl = WishListUrlProvider.GetSignInUrl(new GetWishListUrlParam
            {
                BaseUrl = param.BaseUrl,
                CultureInfo = param.CultureInfo
            });

            return viewModel;
        }
    }
}