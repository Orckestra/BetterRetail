using System;
using System.Collections.Generic;

namespace Orckestra.Composer.Services.Cookie
{
    public class ComposerCookieDto
    {
        /// <summary>
        /// @see ICookieAccessor.Read()
        /// 
        /// You must never create a CookieDto! Only the CookieAccesser should be allowed to do so
        /// 
        /// Remember to always Read before Write
        /// </summary>
        internal ComposerCookieDto()
        {
        }

        [Obsolete("We do not save scope in the cookie for multi scope sites.")]
        public string Scope { get; set; }

        public string EncryptedCustomerId { get; set; }
        public bool? IsGuest { get; set; }

        public string EncryptedEditingOrderId { get; set; }

        /// <summary>
        /// This property can be used for custom cookie properties
        /// </summary>
        public Dictionary<string, string> PropertyBag { get; set; }

        public string GetValue(string key)
        {
            string value = null;
            if (PropertyBag?.TryGetValue(key, out value) == true)
                return value;
            return null;
        }

        public void SetOrRemove(string key, string value)
        {
            if (value == null)
            {
                PropertyBag?.Remove(key);
                return;
            }

            PropertyBag = PropertyBag ?? new Dictionary<string, string>();
            PropertyBag[key] = value;
        }

    }
}
