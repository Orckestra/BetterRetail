using System;
using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.ViewModels.Breadcrumb;

namespace Orckestra.Composer.Cart.Services
{
    public class CheckoutBreadcrumbViewService : ICheckoutBreadcrumbViewService
    {
         protected ILocalizationProvider LocalizationProvider { get; private set; }

         public CheckoutBreadcrumbViewService(ILocalizationProvider localizationProvider)
        {
            if (localizationProvider == null) { throw new ArgumentNullException("localizationProvider"); }

            LocalizationProvider = localizationProvider;
        }

        public BreadcrumbViewModel CreateBreadcrumbViewModel(GetCheckoutBreadcrumbParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.CultureInfo == null) { throw new ArgumentException("param.CultureInfo is required"); }

            var vm = new BreadcrumbViewModel
            {
                Items = new List<BreadcrumbItemViewModel>
                {
                    GenerateHomeItem(param.HomeUrl, param.CultureInfo)
                },
                ActivePageName = GenerateActivePageName(param.CultureInfo)
            };

            return vm;
        }

        private BreadcrumbItemViewModel GenerateHomeItem(string homeUrl, CultureInfo cultureInfo)
        {
            return new BreadcrumbItemViewModel
            {
                DisplayName = LocalizationProvider.GetLocalizedString(new GetLocalizedParam()
                {
                    Category    = "General",
                    Key         = "L_Home",
                    CultureInfo = cultureInfo
                }),
                Url = homeUrl
            };
        }

        private string GenerateActivePageName(CultureInfo cultureInfo)
        {
            var leftPart = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category    = "CheckoutProcess",
                Key = "L_OrderConfirmation",
                CultureInfo = cultureInfo
            });

            return leftPart;
        }
    }
}
