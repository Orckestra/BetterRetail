using System;
using System.Web;

namespace Orckestra.Composer.HttpModules
{
    /// <summary>
    /// Brands requests with Orckestra specific HTTP headers.
    /// </summary>
    public class RequestBranderModule : IHttpModule
    {
        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule" />.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
        }

        private void OnBeginRequest(object sender, EventArgs eventArgs)
        {
            AddBrandingHeaders(new HttpContextWrapper(((HttpApplication) sender).Context));
        }

        /// <summary>
        /// Adds custom Orckestra HTTP headers when a request beings.
        /// </summary>
        /// <param name="context">The context.</param>
        private void AddBrandingHeaders(HttpContextWrapper context)
        {
            context.Response.Headers["X-Powered-By"] = "Orckestra";
            context.Response.Headers["X-Orckestra-Commerce"] = ".NET Client";
        }
    }
}
