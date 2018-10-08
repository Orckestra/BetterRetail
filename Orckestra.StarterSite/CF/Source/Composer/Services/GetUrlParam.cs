using System.Globalization;
using System.Web;

namespace Orckestra.Composer.Services
{
    public class GetUrlParam
    {
        /// <summary>
        /// Current HTTP Context.
        /// </summary>
        public HttpContextBase HttpContext { get; set; }

        /// <summary>
        /// Name of the action in the route.
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Name of the controller in the route.
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Culture in the route. If null, current culture will be used.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Attributes to generate with the route. This parameter is optional.
        /// </summary>
        public object RouteAttributes { get; set; }
    }
}
