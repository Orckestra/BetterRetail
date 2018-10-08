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
    }
}
