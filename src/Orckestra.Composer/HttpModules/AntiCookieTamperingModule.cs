using System;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Orckestra.Composer.Services.Cookie;
using Orckestra.Composer.Services;
using System.Web.Mvc;
using Autofac.Integration.Mvc;

namespace Orckestra.Composer.HttpModules
{
    public class AntiCookieTamperingModule: IHttpModule
    {
        public void Init(HttpApplication context)
        {   
            context.PostAuthenticateRequest += OnPostAuthenticateRequest;
        }

        private void OnPostAuthenticateRequest(object sender, EventArgs eventArgs)
        {
            var httpApplication = (HttpApplication) sender;
            var httpContext = new HttpContextWrapper(httpApplication.Context);

            Handle(httpContext);
        }

        private void Handle(HttpContextBase httpContext)
        {
            var shouldHandle = ShouldHandleRequest(httpContext);
            if (!shouldHandle) { return; }

            var cookieHandler = GetCookieHandler(httpContext);
            var isAuth = IsAuthenticated(httpContext);
            var isGuest = IsGuest(cookieHandler);

            //If not mutually exclusive
            if (isAuth == isGuest)
            {
                if (isAuth && isGuest)   
                {
                    //This means you are logged in, but you are a guest. You need to log back in.
                    RemoveAuth(httpContext);
                }
                else //This means you are not a guest and you are not authenticated.
                {
                    RemoveComposerCookie(httpContext);
                    RemoveAuth(httpContext);
                }

                ReloadIfRequired(httpContext);
            }
        }

        private bool ShouldHandleRequest(HttpContextBase httpContext)
        {
            var excluder = (IAntiCookieTamperingExcluder)AutofacDependencyResolver.Current.GetService(typeof(IAntiCookieTamperingExcluder));
            return excluder != null ? excluder.ShouldHandleRequest(httpContext) : true;
        }

        private void RemoveComposerCookie(HttpContextBase httpContext)
        {
            var composerCookieHandler = GetCookieHandler(httpContext);
            composerCookieHandler.Clear();
        }

        private void RemoveAuth(HttpContextBase httpContext)
        {
            if (FormsAuthentication.IsEnabled)
            {
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, string.Empty)
                {
                    Expires = DateTime.UtcNow.AddDays(-30),
                    HttpOnly = true
                };

                httpContext.Response.Cookies.Set(cookie);
            }

            httpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), new string[0]);
        }

        private bool IsAuthenticated(HttpContextBase httpContext)
        {
            var isAuth = httpContext.User != null && httpContext.User.Identity.IsAuthenticated;
            return isAuth;
        }

        private bool IsGuest(ICookieAccessor<ComposerCookieDto> cookieAccessor)
        {
            var dto = cookieAccessor.Read();

            return dto.IsGuest.GetValueOrDefault(true);
        }

        private void ReloadIfRequired(HttpContextBase httpContext)
        {
            if (httpContext.Request.IsAjaxRequest())
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.ResetContent;
                httpContext.Response.Flush();
                httpContext.ApplicationInstance.CompleteRequest();
            }
        }

        public void Dispose() { }

        protected virtual ICookieAccessor<ComposerCookieDto> GetCookieHandler(HttpContextBase httpContext)
        {
            return new ComposerCookieAccessor(httpContext.Request, httpContext.Response);
        }
    }
}
