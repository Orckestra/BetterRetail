using System;
using System.Web;
using System.Web.Mvc;

namespace Orckestra.Composer.HttpModules
{
    public class SecurityModule : IHttpModule
    {
        public virtual void Init(HttpApplication context)
        {
            MvcHandler.DisableMvcResponseHeader = true;

            context.BeginRequest += OnBeginRequest;
            context.EndRequest += OnEndRequest;
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            var httpApplication = (HttpApplication)sender;
            var httpContext = new HttpContextWrapper(httpApplication.Context);

            HandleBeginRequest(httpContext);
        }

        protected virtual void HandleBeginRequest(HttpContextBase httpContext)
        {
        }


        private void OnEndRequest(object sender, EventArgs e)
        {
            var httpApplication = (HttpApplication)sender;
            var httpContext = new HttpContextWrapper(httpApplication.Context);

            HandleEndRequest(httpContext);
        }

        protected virtual void HandleEndRequest(HttpContextBase httpContext)
        {
            RemoveUselessHeaders(httpContext);
        }

        protected void RemoveUselessHeaders(HttpContextBase httpContext)
        {
            httpContext.Response.Headers.Remove("X-AspNet-Version");
            httpContext.Response.Headers.Remove("Server");
            httpContext.Response.Headers.Remove("Engine");
            httpContext.Response.Headers.Remove("DAV");
        }



        public void Dispose()
        {
        }
    }
}
