using System;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Orckestra.Composer;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;

namespace Composer.LoadTest.Cookie
{
    public class TestCookieAccessor : ICookieAccessor<TestCookieDto>
    {
        private const int Version = 1;

        //Dependencies
        private readonly HttpRequestBase _httpRequest;
        private readonly HttpResponseBase _httpResponse;

        //Configurations
        private readonly string _cookieName;

        private JsonSerializerSettings _jsonSettings;

        private static readonly char[] ValidSeparators = ",|;:!".ToCharArray();
        private static readonly string ExposedSeparator = "|";

        private static readonly EncryptionUtility EncryptionUtility = new EncryptionUtility();

        public TestCookieAccessor(HttpRequestBase httpRequest, HttpResponseBase httpResponse)
        {
            _httpRequest = httpRequest;
            _httpResponse = httpResponse;

            _cookieName = ".Test";
            SetJsonSettings();
        }

        private void SetJsonSettings()
        {
            _jsonSettings = new JsonSerializerSettings
            {
                //Omit null so we don't pollute the cookie
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,

                //Encode date for webfarms
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,

                //Allows for internal constructor
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };
        }

        public TestCookieDto Read()
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
            TestCookieDto dto;

            if (version == 1)
            {
                dto = UnprotectV1(protectedPayload);
            }
            else
            {
                //Fall back to an empty cookie (or set default values)
                var defaultInventoryLocationId = "310";
                dto = new TestCookieDto { ScopeAndLocationId = defaultInventoryLocationId };
            }

            return dto;
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

        public void Write(TestCookieDto dto)
        {
            string protectedPayload = ProtectV1(dto);

            //Add the version
            //Note: the version is not part of the protected payload, this way we can upgrade
            //the payload structure or the protection mecanisme and still know version to use for unprotecting
            string cookieValue = string.Join(ExposedSeparator, Version.ToString(), protectedPayload);

            //Write to cookie
            HttpCookie cookie = new HttpCookie(_cookieName, cookieValue) {HttpOnly = true};
            _httpResponse.Cookies.Set(cookie);
        }

        public void Clear()
        {
            var cookie = new HttpCookie(_cookieName, "")
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(-30)
            };

            _httpResponse.Cookies.Set(cookie);
        }

        #region V1
        private string ProtectV1(TestCookieDto dto)
        {
            //Serialize
            string json = JsonConvert.SerializeObject(dto, _jsonSettings);

            return EncryptionUtility.Encrypt(json);
        }

        private TestCookieDto UnprotectV1(string base64)
        {
            TestCookieDto dto;

            try
            {
                var json = EncryptionUtility.Decrypt(base64);

                //Deserialize
                dto = JsonConvert.DeserializeObject<TestCookieDto>(json, _jsonSettings);
            }
            catch (Exception)
            {
                //Eiher someone tampered with his cookie
                //Or the Encryption method changed (machine key changed?)
                //Or the Deserialize changed
                dto = JsonConvert.DeserializeObject<TestCookieDto>("{}");
            }

            return dto;
        }
        #endregion
    }
}
