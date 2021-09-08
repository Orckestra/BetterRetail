using Orckestra.Composer.CompositeC1.Context;
using Orckestra.Composer.Services;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Store.Providers;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels.LanguageSwitch;
using Orckestra.ExperienceManagement.Configuration;
using System;
using System.Globalization;
using System.Web;

namespace Orckestra.Composer.CompositeC1.Providers.LanguageSwitch
{
    public class StoreLanguageSwitchProvider : ILanguageSwitchProvider
    {
        protected ILanguageSwitchService LanguageSwitchService { get; private set; }
        protected IComposerContext ComposerContext { get; }
        protected IStoreContext StoreContext { get; private set; }
        protected IStoreUrlProvider StoreUrlProvider { get; private set; }
        public ISiteConfiguration SiteConfiguration { get; set; }
        protected HttpRequestBase Request { get; }

        public StoreLanguageSwitchProvider(ILanguageSwitchService languageSwitchViewService,
            IComposerContext composerContext,
            IStoreContext storeContext,
            HttpRequestBase request,
            IStoreUrlProvider storeUrlProvider,
            ISiteConfiguration siteConfiguration)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            LanguageSwitchService = languageSwitchViewService ?? throw new ArgumentNullException(nameof(languageSwitchViewService));
            StoreContext = storeContext ?? throw new ArgumentNullException(nameof(storeContext));
            StoreUrlProvider = storeUrlProvider ?? throw new ArgumentNullException(nameof(storeUrlProvider));
            SiteConfiguration = siteConfiguration ?? throw new ArgumentNullException(nameof(siteConfiguration));
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }

        public bool isActiveForCurrentPage(Guid currentPageId, Guid currentHomePageId)
        {
            var pages = SiteConfiguration.GetPagesConfiguration(ComposerContext.CultureInfo, currentHomePageId);
            return pages.StorePageId == currentPageId;
        }

        public LanguageSwitchViewModel GetViewModel(Guid currentPageId, Guid currentHomePageId)
        {
            var model = StoreContext.ViewModel;
            var baseUrl = RequestUtils.GetBaseUrl(Request).ToString();

            return LanguageSwitchService.GetViewModel(cultureInfo => BuildUrl(
                baseUrl,
                cultureInfo,
                currentHomePageId,
                model.LocalizedDisplayNames[cultureInfo.Name],
                model.Number),
                ComposerContext.CultureInfo);
        }

        protected virtual string BuildUrl(string baseUrl, CultureInfo cultureInfo, Guid currentHomePageId, string storeName, string storeNumber)
        {
            return StoreUrlProvider.GetStoreUrl(new GetStoreUrlParam
            {
                StoreNumber = storeNumber,
                CultureInfo = cultureInfo,
                BaseUrl = baseUrl,
                StoreName = storeName,
                WebsiteId = currentHomePageId
            });
        }
    }
}
