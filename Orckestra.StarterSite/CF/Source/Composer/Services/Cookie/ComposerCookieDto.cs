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

        public string Scope { get; set; }

        public string EncryptedCustomerId { get; set; }
        public bool? IsGuest { get; set; }

        /// <summary>
        /// This property can be used for custom cookie properties
        /// </summary>
        public Dictionary<string, string> PropertyBag { get; set; }
    }
}
