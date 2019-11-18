using System;
using System.Globalization;
using System.Threading;
using System.Web;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services.Cookie;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.Services
{
    //TODO: Rename to ComposerRequestContext
    public class ComposerContext : IComposerContext
    {
        protected ICookieAccessor<ComposerCookieDto> CookieAccessor { get; }
        protected IScopeProvider ScopeProvider { get; }
        protected HttpContextBase HttpContextBase { get; }
        protected ICountryCodeProvider CountryCodeProvider { get; }
        protected IWebsiteContext WebsiteContext { get; }
        protected EncryptionUtility EncryptionUtility { get; }

        public ComposerContext(
            ICookieAccessor<ComposerCookieDto> cookieAccessor,
            IScopeProvider scopeProvider,
            HttpContextBase httpContextBase,
            ICountryCodeProvider countryCodeProvider,
            IWebsiteContext websiteContext)
        {
            CookieAccessor = cookieAccessor ?? throw new ArgumentNullException(nameof(cookieAccessor));
            ScopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
            HttpContextBase = httpContextBase ?? throw new ArgumentNullException(nameof(httpContextBase));
            CountryCodeProvider = countryCodeProvider ?? throw new ArgumentNullException(nameof(countryCodeProvider));
            WebsiteContext = websiteContext ?? throw new ArgumentNullException(nameof(websiteContext));
            EncryptionUtility = new EncryptionUtility();

            SetAuthenticated();
        }

        /// <summary>
        /// The Country
        /// </summary>
        public string CountryCode
        {
            get { return CountryCodeProvider.CountryCode; }
        }

        /// <summary>
        /// Get the currently set Culture
        /// </summary>
        public virtual CultureInfo CultureInfo
        {
            get
            {
                if (_cultureInfo == null)
                {
                    //First attempt, get from path
                    //TODO don't read from Thread, possibly from path or from sitecore context. 
                    //TODO (From Path? From sitecore context? We are in Composer 02/06/15)
                }
                if (_cultureInfo == null)
                {
                    //Second attempt, get from current thread
                    _cultureInfo = Thread.CurrentThread.CurrentCulture;
                }

                return _cultureInfo;
            }
            set
            {
                _cultureInfo = value;
                Thread.CurrentThread.CurrentCulture = _cultureInfo;
                Thread.CurrentThread.CurrentUICulture = _cultureInfo;
            }
        }
        private CultureInfo _cultureInfo = null;

        /// <summary>
        /// Get the selected scope
        /// </summary>
        public virtual string Scope
        {
            get
            {
                if (_scope == null)
                {
                    _scope = ScopeProvider.DefaultScope;
                }

                return _scope;
            }
            set
            {
                _scope = value;
            }
        }
        private string _scope = null;

        /// <summary>
        /// Get the currently connected CustomerID
        /// this info is found in the PayloadCookie
        /// 
        /// If no CustomerID was ever set, a new guest one will be created.
        /// Delay as lat as possible this creation to avoid CustomerID trashing.
        /// </summary>
        public virtual Guid CustomerId
        {
            get
            {
                InitializeCustomerId();

                // ReSharper disable once PossibleInvalidOperationException
                //We initialize the customer id before so it will always return a value.
                //We don't want to return .GetValueOrDefault because the default is like it would be a new customer
                return _customerId.Value;
            }
            set
            {
                if (value == Guid.Empty)
                {
                    _customerId = Guid.NewGuid();
                }
                else
                {
                    _customerId = value;
                }

                //Store it in cookie for later
                ComposerCookieDto dto = CookieAccessor.Read();
                dto.EncryptedCustomerId = EncryptionUtility.Encrypt(_customerId.ToString());
                CookieAccessor.Write(dto);
            }
        }
        private Guid? _customerId = null;

        /// <summary>
        /// Is the CustomerId related to a guest account?
        /// 
        /// This info is stored in the cookie
        /// By default, a CustomerId is considered as a guest until otherwise stated
        /// No values means guest.
        /// </summary>
        public virtual bool IsGuest
        {
            get
            {
                if (!_isGuest.HasValue)
                {
                    //First attempt, lazy load from cookie
                    ComposerCookieDto dto = CookieAccessor.Read();
                    _isGuest = dto.IsGuest;
                }

                if (!_isGuest.HasValue)
                {
                    //Second attempt, if it's not in the cookie it's most likely a guest.
                    _isGuest = true;

                    //no need to store that value
                }

                return _isGuest.Value;
            }
            set
            {
                _isGuest = value;

                //Store it in cookie for later
                ComposerCookieDto dto = CookieAccessor.Read();
                dto.IsGuest = _isGuest;
                CookieAccessor.Write(dto);
            }
        }
        private bool? _isGuest = null;

        public virtual bool IsAuthenticated
        {
            get
            {
                return _isAuthenticatedLazy.Value;
            }
        }
        private Lazy<bool> _isAuthenticatedLazy;


        /// <summary>
        /// Gets the encrypted customer identifier.
        /// </summary>
        /// <returns></returns>
        public virtual string GetEncryptedCustomerId()
        {
            InitializeCustomerId();
            ComposerCookieDto dto = CookieAccessor.Read();

            return dto.EncryptedCustomerId;
        }

        /// <summary>
        /// Initializes the customer identifier.
        /// </summary>
        private void InitializeCustomerId()
        {
            if (!_customerId.HasValue)
            {
                //First attempt, lazy load from cookie
                var dto = CookieAccessor.Read();

                if (dto.EncryptedCustomerId != null)
                {
                    _customerId = new Guid(EncryptionUtility.Decrypt(dto.EncryptedCustomerId));
                }
                else
                {
                    //Second attempt, create a new customerId
                    _customerId = Guid.NewGuid();
                    _isGuest = true;

                    //Store it in cookie for later
                    dto.EncryptedCustomerId = EncryptionUtility.Encrypt(_customerId.ToString());
                    dto.IsGuest = _isGuest;
                    CookieAccessor.Write(dto);
                }
            }
        }

        private void SetAuthenticated()
        {
            var composerContext = CookieAccessor.Read();

            if (composerContext.EncryptedCustomerId == null)
            {
                _isAuthenticatedLazy = new Lazy<bool>(() => false);
            }
            else
            {
                _isAuthenticatedLazy = new Lazy<bool>(() =>
                {
                    var websiteId = (HttpContextBase.User?.Identity as System.Web.Security.FormsIdentity)?.Ticket?.UserData;
                    bool isCurrentWebsite = websiteId == WebsiteContext.WebsiteId.ToString();
                    bool IsAuthenticated = HttpContextBase.User?.Identity.IsAuthenticated ?? false;
                    return isCurrentWebsite && IsAuthenticated;
                });
            }
        }
    }
}
