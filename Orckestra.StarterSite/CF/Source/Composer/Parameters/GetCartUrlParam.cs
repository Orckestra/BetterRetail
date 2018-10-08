using System.Globalization;

namespace Orckestra.Composer.Parameters
{
    public class GetCartUrlParam
    {
        /// <summary>
        /// The culture info in which language the data will be returned
        /// Optional
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// ReturnUrl to preserve
        /// Optional
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}
