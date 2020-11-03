using Orckestra.Composer.Cart;
using Orckestra.Composer.Grocery.Parameters;
using Orckestra.Composer.Grocery.Providers;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using System;
using System.Threading.Tasks;

namespace Orckestra.Composer.Grocery.Services
{
    public class GroceryMembershipViewService : MembershipViewService
    {
        public GroceryMembershipViewService(
            IMyAccountUrlProvider myAccountUrlProvider,
            IViewModelMapper viewModelMapper,
            ICustomerRepository customerRepository,
            ICartMergeProvider cartMergeProvider,
            IStoreAndFulfillmentSelectionProvider storeAndFulfillmentSelectionProvider,
            IRegexRulesProvider regexRulesProvider,
            IComposerContext composerContext)
            : base(myAccountUrlProvider, viewModelMapper, customerRepository, cartMergeProvider, composerContext, regexRulesProvider)
        {
            StoreAndFulfillmentSelectionProvider = storeAndFulfillmentSelectionProvider ?? throw new ArgumentNullException(nameof(storeAndFulfillmentSelectionProvider));
        }
        public IStoreAndFulfillmentSelectionProvider StoreAndFulfillmentSelectionProvider { get; }

        public override async Task<LoginViewModel> LoginAsync(LoginParam param)
        {
            var loginViewModel = await base.LoginAsync(param).ConfigureAwait(false);
            await StoreAndFulfillmentSelectionProvider.RecoverSelection(new RecoverSelectionDataParam
            {
                CustomerId = loginViewModel.CustomerId,
                CultureInfo = ComposerContext.CultureInfo,
                IsAuthenticated = ComposerContext.IsAuthenticated,
                CartName = CartConfiguration.ShoppingCartName
            }).ConfigureAwait(false);

            return loginViewModel;
        }
        public override void LogOutCustomer()
        {
            StoreAndFulfillmentSelectionProvider.ClearSelection();
            base.LogOutCustomer();
        }
    }
}