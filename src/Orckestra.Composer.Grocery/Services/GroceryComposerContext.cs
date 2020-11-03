using Orckestra.Composer.Grocery.Parameters;
using Orckestra.Composer.Grocery.Providers;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Cookie;
using System;
using System.Web;

namespace OOrckestra.Composer.Grocery.Services
{
    public class GroceryComposerContext : ComposerContext
    {
        protected IStoreAndFulfillmentSelectionProvider StoreAndFulfillmentSelectionProvider { get; }
        public GroceryComposerContext(
            ICookieAccessor<ComposerCookieDto> cookieAccessor,
            IScopeProvider scopeProvider,
            HttpContextBase httpContextBase,
            ICountryCodeProvider countryCodeProvider,
            IWebsiteContext websiteContext,
            IStoreAndFulfillmentSelectionProvider storeAndFulfillmentSelectionProvider) : base(cookieAccessor, scopeProvider, httpContextBase, countryCodeProvider, websiteContext)
        {
            StoreAndFulfillmentSelectionProvider = storeAndFulfillmentSelectionProvider ?? throw new ArgumentNullException(nameof(storeAndFulfillmentSelectionProvider));
        }

        /// <summary>
        /// Get the selected scope
        /// </summary>
        public override string Scope
        {
            get
            {
                if (_scope == null)
                {
                    var store = StoreAndFulfillmentSelectionProvider.GetSelectedStoreAsync(new GetSelectedFulfillmentParam
                    {
                        CustomerId = CustomerId,
                        IsAuthenticated = IsAuthenticated,
                        CultureInfo = CultureInfo,
                        TryGetFromDefaultSettings = true
                    }).Result;

                    _scope = string.IsNullOrWhiteSpace(store?.ScopeId) ? ScopeProvider.DefaultScope : store.ScopeId;
                }

                return _scope;
            }
            set
            {
                _scope = value;
            }
        }
        private string _scope = null;

    }
}
