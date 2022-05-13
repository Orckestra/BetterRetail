using System;
using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Cart.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.ViewModels.Breadcrumb;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Cart.Services
{
    public class CheckoutBreadcrumbViewService : ICheckoutBreadcrumbViewService
    {
         protected ILocalizationProvider LocalizationProvider { get; private set; }

         public CheckoutBreadcrumbViewService(ILocalizationProvider localizationProvider)
        {
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
        }

        public virtual BreadcrumbViewModel CreateBreadcrumbViewModel(GetCheckoutBreadcrumbParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }

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

        protected virtual BreadcrumbItemViewModel GenerateHomeItem(string homeUrl, CultureInfo cultureInfo)
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

        protected virtual string GenerateActivePageName(CultureInfo cultureInfo)
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
