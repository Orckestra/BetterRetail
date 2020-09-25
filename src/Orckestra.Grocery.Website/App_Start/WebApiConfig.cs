using System.Net.Http.Formatting;
using System.Web.Http;

namespace Orckestra.Composer.Grocery.Website
{
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers WebAPI Routes and configs
        /// </summary>
        /// <param name="config">Asp configuration to update.</param>
        public static void Register(HttpConfiguration config)
        {
            //Web API configuration and servicse
            config.Formatters.XmlFormatter.AddUriPathExtensionMapping("xml", XmlMediaTypeFormatter.DefaultMediaType);
            config.Formatters.JsonFormatter.AddUriPathExtensionMapping("json", JsonMediaTypeFormatter.DefaultMediaType);

            // Web API routes

            // Default conventional routes
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
