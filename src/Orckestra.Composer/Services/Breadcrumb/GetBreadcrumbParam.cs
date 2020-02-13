using System.Globalization;

namespace Orckestra.Composer.Services.Breadcrumb
{
    public class GetBreadcrumbParam
    {
        /// <summary>
        /// The cultulre Info
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// The Id of the current Page
        /// </summary>
        public string CurrentPageId { get; set; }
    }
}
