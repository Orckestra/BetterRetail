using System;
using System.Globalization;
using System.Threading;
using System.Web;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Services.Cookie;
using Orckestra.Composer.Utils;
using Orckestra.ExperienceManagement.Configuration;

namespace Orckestra.Composer.Services
{
	//TODO: Rename to ComposerRequestContext
	//TODO: Maybe refactor setter getter with side-effects to methods
	public class ComposerContext : IComposerContext
	{
		private readonly ICookieAccessor<ComposerCookieDto> _cookieAccessor;
		private readonly IScopeProvider _scopeProvider;
        private readonly ICountryCodeProvider _countryCodeProvider;
        private readonly HttpContextBase _httpContextBase;
		private readonly EncryptionUtility _encryptionUtility = new EncryptionUtility();

		public ComposerContext(
			ICookieAccessor<ComposerCookieDto> cookieAccessor, 
			IScopeProvider scopeProvider,
			HttpContextBase httpContextBase,
            ICountryCodeProvider countryCodeProvider)
		{
			_cookieAccessor = cookieAccessor;
			_scopeProvider = scopeProvider;
			_httpContextBase = httpContextBase;
            _countryCodeProvider = countryCodeProvider;


            SetAuthenticated();
		}

		/// <summary>
		/// The Country
		/// </summary>
		public string CountryCode
		{
			get { return _countryCodeProvider.CountryCode; }
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
                    _scope = _scopeProvider.DefaultScope;
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
				ComposerCookieDto dto = _cookieAccessor.Read();
				dto.EncryptedCustomerId = _encryptionUtility.Encrypt(_customerId.ToString());
				_cookieAccessor.Write(dto);
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
					ComposerCookieDto dto = _cookieAccessor.Read();
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
				ComposerCookieDto dto = _cookieAccessor.Read();
				dto.IsGuest = _isGuest;
				_cookieAccessor.Write(dto);
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
			ComposerCookieDto dto = _cookieAccessor.Read();

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
				var dto = _cookieAccessor.Read();

			    if (dto.EncryptedCustomerId != null)
			    {
			        _customerId = new Guid(_encryptionUtility.Decrypt(dto.EncryptedCustomerId));
			    }
			    else
			    {
			        //Second attempt, create a new customerId
			        _customerId = Guid.NewGuid();
			        _isGuest = true;

			        //Store it in cookie for later
			        dto.EncryptedCustomerId = _encryptionUtility.Encrypt(_customerId.ToString());
			        dto.IsGuest = _isGuest;
			        _cookieAccessor.Write(dto);
			    }
			}
		}

		private void SetAuthenticated()
		{
			var composerContext = _cookieAccessor.Read();

			if (composerContext.EncryptedCustomerId == null)
			{
				_isAuthenticatedLazy = new Lazy<bool>(() => false);
			}
			else
			{
				_isAuthenticatedLazy = new Lazy<bool>(() => _httpContextBase.User != null && _httpContextBase.User.Identity.IsAuthenticated);
			}
		}
	}
}
