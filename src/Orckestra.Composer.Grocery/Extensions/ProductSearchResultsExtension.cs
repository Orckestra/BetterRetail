using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orckestra.Composer.Enums;
using Orckestra.Composer.Grocery.ViewModels;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Services;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services.Lookup;

namespace Orckestra.Composer.Grocery.Extensions
{
    public static class ProductSearchResultsExtension
    {
        public static async Task BuildProductBadgeValues<TParam>(this ProductSearchViewModel productSearchViewModel, CreateProductSearchResultsViewModelParam<TParam> createSearchViewModelParam, ILookupService lookupService, string cultureName)
        {
            var extendedVM = productSearchViewModel.AsExtensionModel<IGroceryProductSearchViewModel>();
            if (extendedVM.ProductBadges == null)
            { return; }

            IDictionary<string, string> productBadgesLookupValueDictionary = new Dictionary<string, string>();

            var productLookups = await lookupService.GetLookupsAsync(LookupType.Product);
            productLookups
                .FirstOrDefault(item => item.LookupName == "ProductBadges")
                .Values
                .ForEach(item => productBadgesLookupValueDictionary.Add(item.DisplayName.GetLocalizedValue(cultureName), item.Value));

            extendedVM.ProductBadgeValues = new Dictionary<string, string>();
            foreach (var extendedVmProductBadge in extendedVM.ProductBadges)
            {
                if (productBadgesLookupValueDictionary.ContainsKey(extendedVmProductBadge))
                    extendedVM.ProductBadgeValues.Add(productBadgesLookupValueDictionary[extendedVmProductBadge], extendedVmProductBadge);
            }

            productSearchViewModel.Context["ProductBadgeValues"] = extendedVM.ProductBadgeValues;
        }
    }
}
