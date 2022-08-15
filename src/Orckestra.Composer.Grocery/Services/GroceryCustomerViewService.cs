using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Enums;
using Orckestra.Composer.MyAccount.Parameters;
using Orckestra.Composer.MyAccount.ViewModels;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.ViewModels;
using Orckestra.Composer.MyAccount.Services;
using Orckestra.Composer.Grocery.ViewModels;
using Orckestra.Composer.Services.Lookup;

namespace Orckestra.Composer.Grocery.Services
{
    public class GroceryCustomerViewService : CustomerViewService
    {
        public ILookupService LookupService { get; set; }
        public IComposerContext ComposerContext { get; set; }

        private const string CustomProfileSubstitutionOptions = "SubstitutionOptions";

        public GroceryCustomerViewService(
            IViewModelMapper viewModelMapper,
            ICustomerRepository customerRepository,
            ICultureService cultureService,
            ILocalizationProvider localizationProvider,
            IRegexRulesProvider regexRulesProvider,
            ILookupService lookupService,
            IMyAccountUrlProvider myAccountUrlProvider,
            IComposerContext composerContext) : base(
            viewModelMapper,
            customerRepository,
            cultureService,
            localizationProvider,
            regexRulesProvider,
            myAccountUrlProvider)
        {
            LookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
        }

        public override async Task<UpdateAccountViewModel> GetUpdateAccountViewModelAsync(GetUpdateAccountViewModelParam param)
        {
            var resultViewModel = await base.GetUpdateAccountViewModelAsync(param).ConfigureAwait(false);
            var extendedViewModel = resultViewModel.AsExtensionModel<IGroceryUpdateAccountViewModel>();

            extendedViewModel.SubstitutionOptions = await GetDefaultSubstitutionOptionsList().ConfigureAwait(false);

            return resultViewModel;
        }

        private async Task<Dictionary<string, string>> GetDefaultSubstitutionOptionsList()
        {
            var lookup = await LookupService.GetLookupAsync(LookupType.Customer, CustomProfileSubstitutionOptions).ConfigureAwait(false);

            var substitutionOptions = lookup.Values.OrderBy(lookupValue => lookupValue.SortOrder)
                .Where(lookupValue => lookupValue.IsActive)
                .ToDictionary(key => key.Value, value => value.DisplayName.GetLocalizedValue(ComposerContext.CultureInfo.Name));

            return substitutionOptions;
        }
    }
}
