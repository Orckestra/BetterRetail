using System.Globalization;
using Orckestra.Composer.Providers;

namespace Orckestra.Composer.Parameters
{
    /// <summary>
    /// Parameter for <see cref="IMyAccountUrlProvider" />
    /// </summary>
    public class GetMyAccountUrlParam
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
