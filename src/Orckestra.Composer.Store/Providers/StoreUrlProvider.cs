using System;
using System.Collections.Specialized;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Providers.Localization;
using Orckestra.Composer.Store.Parameters;
using Orckestra.Composer.Utils;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Store.Providers
{
    public class StoreUrlProvider: IStoreUrlProvider
    {
        private const string UrlTemplate = "/{0}/{1}/{2}-s-{3}";
        private const string UrlStoreLocatorTemplate = "/{0}/{1}";
        private const string UrlStoresDirectoryTemplate = "/{0}/{1}";
        protected const string ResourceCategory = "Mvc-Cms";
        protected ILocalizationProvider LocalizationProvider { get; private set; }

        public StoreUrlProvider(ILocalizationProvider localizationProvider)
        {
            LocalizationProvider = localizationProvider ?? throw new ArgumentNullException(nameof(localizationProvider));
        }

        public virtual string GetStoreLocatorUrl(GetStoreLocatorUrlParam parameters)
        {
            var storePath = string.Format(UrlStoreLocatorTemplate,
                parameters.CultureInfo.Name,
                LocalizationProvider.GetLocalizedString(new GetLocalizedParam
                {
                    Category = ResourceCategory,
                    Key = "Stores_Url",
                    CultureInfo = parameters.CultureInfo
                })
            );

            var uri = new Uri(storePath, UriKind.Relative);

            var queryString = new NameValueCollection();

            if(parameters.Page != 1)
            {
                queryString.Add("page", parameters.Page.ToString());
            }        

            return UrlFormatter.AppendQueryString(uri.ToString(), queryString);
        }

        public virtual string GetStoreUrl(GetStoreUrlParam parameters)
        {
            if (parameters == null) { throw new ArgumentNullException(nameof(parameters)); }
            if (string.IsNullOrWhiteSpace(parameters.BaseUrl)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(parameters.BaseUrl)), nameof(parameters)); }
            if (parameters.CultureInfo == null) { throw new ArgumentException(GetMessageOfNull(nameof(parameters.CultureInfo)), nameof(parameters)); }

            var storePath = string.Format(UrlTemplate,
                parameters.CultureInfo.Name,
                LocalizationProvider.GetLocalizedString(new GetLocalizedParam
                {
                    Category = ResourceCategory,
                    Key = "Stores_Url",
                    CultureInfo = parameters.CultureInfo
                }),
                parameters.StoreNumber,
                UrlFormatter.Format(parameters.StoreName));

            var uri = new Uri(storePath, UriKind.Relative);

            return uri.ToString();
        }

        public virtual string GetStoresDirectoryUrl(GetStoresDirectoryUrlParam parameters)
        {
            var storeDirectoryPath = string.Format(UrlStoresDirectoryTemplate,
              parameters.CultureInfo.Name,
              LocalizationProvider.GetLocalizedString(new GetLocalizedParam
              {
                  Category = ResourceCategory,
                  Key = "StoresDirectory_Url",
                  CultureInfo = parameters.CultureInfo
              })
          );

            var uri = new Uri(storeDirectoryPath, UriKind.Relative);
            //var uri = new Uri(
            //   new Uri(parameters.BaseUrl, UriKind.Absolute),
            //   new Uri(storeDirectoryPath, UriKind.Relative));
            var queryString = new NameValueCollection();
            if (parameters.Page != 1)
                queryString.Add("page", parameters.Page.ToString());

            return UrlFormatter.AppendQueryString(uri.ToString(), queryString);
        }
    }
}