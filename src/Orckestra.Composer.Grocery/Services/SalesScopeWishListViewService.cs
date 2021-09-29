using Orckestra.Composer.Cart.Factory;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Cart.Providers.WishList;
using Orckestra.Composer.Cart.Repositories;
using Orckestra.Composer.Cart.Services;
using Orckestra.Composer.Cart.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Overture.ServiceModel.Orders;
using System;
using System.Threading.Tasks;
using Orckestra.Composer.Repositories;

namespace Orckestra.Composer.Grocery.Services
{
    public class SalesScopeWishListViewService: WishListViewService
    {
        private readonly IScopeRepository _scopeRepository;
        public SalesScopeWishListViewService(
             IWishListRepository wishListRepository,
             ILineItemViewModelFactory lineItemViewModelFactory,
             IWishListUrlProvider wishListUrlProvider,
             IFixCartService fixCartService,
             IImageService imageService,
             IScopeRepository scopeRepository)
             : base(wishListRepository, lineItemViewModelFactory, wishListUrlProvider, fixCartService, imageService)
        {
            _scopeRepository = scopeRepository ?? throw new ArgumentNullException(nameof(scopeRepository));
        }
        public override async Task<WishListSummaryViewModel> AddLineItemAsync(AddLineItemParam param)
        {
            param.Scope = await _scopeRepository.GetSaleScopeAsync(param.Scope).ConfigureAwait(false);
            return await base.AddLineItemAsync(param).ConfigureAwait(false);
        }

        public override async Task<WishListSummaryViewModel> RemoveLineItemAsync(RemoveLineItemParam param)
        {
            param.Scope = await _scopeRepository.GetSaleScopeAsync(param.Scope).ConfigureAwait(false);
            return await base.RemoveLineItemAsync(param).ConfigureAwait(false);
        }

        public override async Task<WishListViewModel> GetWishListViewModelAsync(GetCartParam param)
        {
            param.Scope = await _scopeRepository.GetSaleScopeAsync(param.Scope).ConfigureAwait(false);
            return await base.GetWishListViewModelAsync(param).ConfigureAwait(false);
        }


        public override async Task<WishListSummaryViewModel> GetWishListSummaryViewModelAsync(GetCartParam param)
        {
            param.Scope = await _scopeRepository.GetSaleScopeAsync(param.Scope).ConfigureAwait(false);
            return await base.GetWishListSummaryViewModelAsync(param).ConfigureAwait(false);
        }

        protected override async Task<WishListViewModel> CreateWishListViewModelAsync(CreateWishListViewModelParam param)
        {
            param.WishList.ScopeId = await _scopeRepository.GetSaleScopeAsync(param.WishList.ScopeId).ConfigureAwait(false);
            return await base.CreateWishListViewModelAsync(param);
        }

        protected override WishListSummaryViewModel CreateSummaryWishListViewModel(CreateWishListViewModelParam param)
        {
            param.WishList.ScopeId = _scopeRepository.GetSaleScopeAsync(param.WishList.ScopeId).Result;
            return base.CreateSummaryWishListViewModel(param);
        }

        protected override Task<ProcessedCart> FixWishList(ProcessedCart wishList)
        {
            return Task.FromResult(wishList);
        }
    }
}
