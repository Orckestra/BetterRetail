using System;
using System.Linq;
using System.Web;
using Autofac.Integration.Mvc;
using Newtonsoft.Json;
using Orckestra.Composer.Logging;
using Orckestra.Composer.Utils;
using Orckestra.ExperienceManagement.Configuration;

namespace Orckestra.Composer.Services.Cookie
{
    /// <summary>
    /// Tamper protected cookie for holding Who the consumer is.
    ///
    /// The DataProtection is done using MachineKeyDataProtector
    /// so make sure all IIS instances have the same machine key configured on your webfarm (which should already be the case for session sharing)
    ///
    /// If you are not If you are not using IIS, there are two possible alternative.
    ///     For single host you can use the System.Security.Cryptography.DpapiDataProtector
    ///     For webfarm not behind iis X509CertificateDataProtector
    /// </summary>
    public class ComposerCookieAccessor: ICookieAccessor<ComposerCookieDto>
    {
        private static ILog Log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// Current version for newly created dto cookies
        /// You might need to change the version if the Dto is changed in a incompatible way (renaming fields)
        /// or if the protectiong mecanisme is changed
        /// </summary>
        private const int Version = 1;

        //Dependencies
        private readonly HttpRequestBase  _httpRequest;
        private readonly HttpResponseBase _httpResponse;
        private readonly IWebsiteContext _websiteContext;
        private readonly ISiteConfiguration _siteConfiguration;

        //Configurations
        private readonly string           _cookieName;
        private readonly string           _cookieDomain;
        private readonly bool             _requireSsl;
        private readonly int              _timeoutInMinutes;

        private readonly JsonSerializerSettings _jsonSettings;

        /// <summary>
        /// The _requestCachedDto is used to reduce the workload on Reading and Unprotecting the cookie
        /// Once the cookie is read using Read, we keep a reusable instance.
        ///
        /// That instance still need to be protected and written back into the cookie using Write
        /// </summary>
        private ComposerCookieDto _requestCachedDto = null;

        private static readonly char[] ValidSeparators = ",|;:!".ToCharArray();
        private static readonly string ExposedSeparator = "|";

        private static readonly EncryptionUtility EncryptionUtility = new EncryptionUtility();

        public ComposerCookieAccessor(HttpRequestBase httpRequest, HttpResponseBase httpResponse)
        {
            _httpRequest = httpRequest ?? throw new ArgumentNullException(nameof(httpRequest));
            _httpResponse = httpResponse ?? throw new ArgumentNullException(nameof(httpResponse));

            _websiteContext =
                (IWebsiteContext) AutofacDependencyResolver.Current.GetService(typeof(IWebsiteContext));
            _siteConfiguration =
               (ISiteConfiguration)AutofacDependencyResolver.Current.GetService(typeof(ISiteConfiguration));

            _cookieName = _siteConfiguration.CookieAccesserSettings.Name + "_" + _websiteContext.WebsiteId;
            _requireSsl       = _siteConfiguration.CookieAccesserSettings.RequireSsl;
            _timeoutInMinutes = _siteConfiguration.CookieAccesserSettings.TimeoutInMinutes;
            _cookieDomain     = _siteConfiguration.CookieAccesserSettings.Domain;

            _jsonSettings = new JsonSerializerSettings
            {
                //Omit null so we don't pollute the cookie
                NullValueHandling     = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,

                //Encode date for webfarms
                DateFormatHandling   = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,

                //Allows for internal constructor
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };
        }

        /// <summary>
        /// Read the data from the Request Cookie.
        /// </summary>
        /// <returns>The loaded Data transfer object</returns>
        public ComposerCookieDto Read()
        {
            if (_requestCachedDto == null)
            {
                HttpCookie cookie = GetCookie();

                string cookieValue = string.Empty;
                if (cookie != null)
                {
                    cookieValue = cookie.Value;
                }

                string[] pack = cookieValue.Split(ValidSeparators, 2, StringSplitOptions.None);
                //Extract back the version (Item1)
                if (!int.TryParse(pack.FirstOrDefault() ?? string.Empty, out int version))
                {
                    version = 0;
                }
                //Extract back the protectedPayload (Item2)
                string protectedPayload = pack.Skip(1).FirstOrDefault() ?? string.Empty;


                //Use the appropriate version to unprotect and deserialize the payload
                ComposerCookieDto dto;
                if (version == 1)
                {
                    dto = UnprotectV1(protectedPayload);
                }
                else
                {
                    //Fall back to an empty cookie (or set default values)
                    dto = new ComposerCookieDto();
                }

                _requestCachedDto = dto;
            }

            return _requestCachedDto;
        }

        private HttpCookie GetCookie()
        {
            var cookie = GetCookieFromResponse() ?? GetCookieFromRequest();
            return cookie;
        }

        private HttpCookie GetCookieFromResponse()
        {
            if (_httpResponse.Cookies.AllKeys.Contains(_cookieName))
            {
                HttpCookie cookie = _httpResponse.Cookies.Get(_cookieName);
                return cookie;
            }

            return null;
        }

        private HttpCookie GetCookieFromRequest()
        {
            HttpCookie cookie = _httpRequest.Cookies.Get(_cookieName);
            return cookie;
        }

        /// <summary>
        /// Write the dto to the Response Cookie, overriding any existing data
        /// For updating, it is mandatory to Read first, update, and the store everything
        /// </summary>
        public void Write(ComposerCookieDto dto)
        {
            string protectedPayload = ProtectV1(dto);

            //Add the version
            //Note: the version is not part of the protected payload, this way we can upgrade
            //the payload structure or the protection mecanisme and still know version to use for unprotecting
            string cookieValue = string.Join(ExposedSeparator, Version.ToString(), protectedPayload);

            //Write to cookie
            HttpCookie cookie = new HttpCookie(_cookieName, cookieValue)
            {
                Secure = _requireSsl,
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(_timeoutInMinutes)
            };
            if (!string.IsNullOrWhiteSpace(_cookieDomain))
            {
                cookie.Domain = _cookieDomain;
            }

            _httpResponse.Cookies.Set(cookie);

            //Update the request cached version
            _requestCachedDto = dto;
        }

        public void Clear()
        {
            var cookie = new HttpCookie(_cookieName, string.Empty)
            {
                HttpOnly = true,
                Secure = _requireSsl,
                Expires = DateTime.UtcNow.AddDays(-30)
            };

            _httpResponse.Cookies.Set(cookie);
            _requestCachedDto = null;
        }

        #region V1
        private string ProtectV1(ComposerCookieDto dto)
        {
            //Serialize
            string json = JsonConvert.SerializeObject(dto, _jsonSettings);

            return EncryptionUtility.Encrypt(json);
        }

        private ComposerCookieDto UnprotectV1(string base64)
        {
            ComposerCookieDto dto;

            try
            {
                var json = EncryptionUtility.Decrypt(base64);
                dto = JsonConvert.DeserializeObject<ComposerCookieDto>(json, _jsonSettings);
            }
            catch (Exception ex)
            {
                var errorMessage =
                    "Either someone tampered with his cookie or the encryption method changed (machine key changed?) or the Deserialize changed";

                Log.ErrorException(errorMessage, ex);
                dto = JsonConvert.DeserializeObject<ComposerCookieDto>("{}");
            }

            return dto;
        }

        #endregion

        #region V2
        // You will eventually need to upgrade the version; that is if the Dto changes in a incompatible way
        //or if the Protection mecanisme changes
        //
        //1) Upgrade the DTO_VERSION
        //2) Implement the ProtectV2 and UnprotectV2
        //3) Implement a DtoV1_to_DtoV2 converter
        //4) in Write, use the ProtectV2
        //5) in Read, use the UnprotectV2
        //6) in Read, if version is one, chaine calls through the converters
        #endregion
    }
}
