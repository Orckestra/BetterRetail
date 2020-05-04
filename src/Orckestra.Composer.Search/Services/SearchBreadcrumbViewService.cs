using System;
using System.Collections.Generic;
using System.Globalization;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.ViewModels.Breadcrumb;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Search.Services
{
    public class SearchBreadcrumbViewService : ISearchBreadcrumbViewService
    {
        protected ILocalizationProvider LocalizationProvider { get; private set; }

        public SearchBreadcrumbViewService(ILocalizationProvider localizationProvider)
        {
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
        }

        public virtual BreadcrumbViewModel CreateBreadcrumbViewModel(GetSearchBreadcrumbParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CultureInfo)), nameof(param)); }

            var vm = new BreadcrumbViewModel
            {
                Items = new List<BreadcrumbItemViewModel>
                {
                    GenerateHomeItem(param.HomeUrl, param.CultureInfo)
                },
                ActivePageName = GenerateActivePageName(param.Keywords, param.CultureInfo)
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

        protected virtual string GenerateActivePageName(string keywords, CultureInfo cultureInfo)
        {
            var leftPart = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category    = "List-Search",
                Key         = "L_SearchResultsForBreadcrumb",
                CultureInfo = cultureInfo
            });

            var quoteOpen = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category    = "General",
                Key         = "L_QuoteOpen",
                CultureInfo = cultureInfo
            });

            var quoteClose = LocalizationProvider.GetLocalizedString(new GetLocalizedParam
            {
                Category    = "General",
                Key         = "L_QuoteClose",
                CultureInfo = cultureInfo
            });

            return string.Format("{0} {1}{2}{3}", leftPart, quoteOpen, keywords ?? string.Empty, quoteClose);
        }
    }
}