using Orckestra.Composer.Cart;
using Orckestra.Composer.CompositeC1.Providers;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Grocery.Parameters;
using Orckestra.Composer.Grocery.Providers;
using Orckestra.Composer.Grocery.Settings;
using Orckestra.Composer.Grocery.ViewModels;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Recipes.Settings;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using System;
using System.Threading.Tasks;

namespace Orckestra.Composer.Grocery.Services
{
    public class GroceryMembershipViewService : MembershipViewService
    {
        public IStoreAndFulfillmentSelectionProvider StoreAndFulfillmentSelectionProvider { get; }
        protected IPageService PageService { get; private set; }
        private IMyUsualsSettings MyUsualsSettings { get; }
        private IRecipeSettings RecipeSettings { get; }

        public GroceryMembershipViewService(
            IMyAccountUrlProvider myAccountUrlProvider,
            IViewModelMapper viewModelMapper,
            ICustomerRepository customerRepository,
            ICartMergeProvider cartMergeProvider,
            IStoreAndFulfillmentSelectionProvider storeAndFulfillmentSelectionProvider,
            IRegexRulesProvider regexRulesProvider,
            IComposerContext composerContext,
            IPageService pageService,
            IMyUsualsSettings myUsualsSettings,
            IRecipeSettings recipeSettings)
            : base(myAccountUrlProvider, viewModelMapper, customerRepository, cartMergeProvider, composerContext, regexRulesProvider)
        {
            StoreAndFulfillmentSelectionProvider = storeAndFulfillmentSelectionProvider ?? throw new ArgumentNullException(nameof(storeAndFulfillmentSelectionProvider));
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            MyUsualsSettings = myUsualsSettings ?? throw new ArgumentNullException(nameof(myUsualsSettings));
            RecipeSettings = recipeSettings ?? throw new ArgumentNullException(nameof(recipeSettings));
        }

        public override async Task<LoginViewModel> LoginAsync(LoginParam param)
        {
            var loginViewModel = await base.LoginAsync(param).ConfigureAwait(false);
            await StoreAndFulfillmentSelectionProvider.RecoverSelection(new RecoverSelectionDataParam
            {
                CustomerId = loginViewModel.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                IsAuthenticated = loginViewModel.IsSuccess,
                CartName = CartConfiguration.ShoppingCartName,
                // remember Guest scope, to be able move cart from this scope
                GuestScope = ComposerContext.Scope
            }).ConfigureAwait(false);

            return loginViewModel;
        }
        public override void LogOutCustomer()
        {
            StoreAndFulfillmentSelectionProvider.ClearSelection();
            base.LogOutCustomer();
        }

        public override async Task<UserMetadataViewModel> GetUserMetadataModel(GetUserMetadataParam param)
        {
            var viewModel = await base.GetUserMetadataModel(param).ConfigureAwait(false);
            var extendedVM = viewModel.AsExtensionModel<IGroceryUserMetadataViewModel>();

            var urlParam = new BaseUrlParameter
            {
                CultureInfo = param.CultureInfo
            };
            extendedVM.MyUsualsUrl = GetUrl(urlParam, MyUsualsSettings.MyUsualsPageId);
            extendedVM.RecipeFavoritesUrl = GetUrl(urlParam, RecipeSettings.RecipeFavoritesPageId);

            return viewModel;
        }

        private string GetUrl(BaseUrlParameter param, Guid pageId)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (pageId == null) { throw new ArgumentNullException(nameof(pageId)); }

            var url = PageService.GetPageUrl(pageId, param.CultureInfo);
            return UrlProviderHelper.BuildUrlWithParams(url, param.ReturnUrl);
        }
    }
}