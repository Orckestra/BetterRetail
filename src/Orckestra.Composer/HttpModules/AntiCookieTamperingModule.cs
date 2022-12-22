using System;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Orckestra.Composer.Services.Cookie;
using Orckestra.Composer.Services;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Utils;
using System.Globalization;
using System.Configuration;

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

        //// ReSharper disable once InconsistentNaming
        //private readonly bool ValidationPasswordChangedNotDisabled =
        //    ConfigurationManager.AppSettings["Orckestra.ComposerContext.DisableValidationPasswordChanged"] != "true";

        private void Handle(HttpContextBase httpContext)
        {
            var shouldHandle = ShouldHandleRequest(httpContext);
            if (!shouldHandle) { return; }

            var cookieHandler = GetCookieHandler(httpContext);
            var isAuth = IsAuthenticated(httpContext);
            var isGuest = IsGuest(cookieHandler);

            //Scope and Culture Info is not available at this time
            //var dto = cookieHandler.Read();

            //if (dto.EncryptedCustomerId != null)
            //{
            //    var decryptedCustomerId = new Guid(new EncryptionUtility().Decrypt(dto.EncryptedCustomerId));

            //    var ComposerContext = (IComposerContext)AutofacDependencyResolver.Current.GetService(typeof(IComposerContext));
            //    var CustomerRepository = (ICustomerRepository)AutofacDependencyResolver.Current.GetService(typeof(ICustomerRepository));

            //    if (dto.IsGuest != true && ValidationPasswordChangedNotDisabled)
            //    {
            //        // GetCustomerByIdAsync uses a cache
            //        var customer = CustomerRepository.GetCustomerByIdAsync(new GetCustomerByIdParam()
            //        {
            //            CustomerId = decryptedCustomerId,
            //            Scope = ComposerContext.Scope,
            //            CultureInfo = ComposerContext.CultureInfo,
            //            IncludeAddresses =
            //                true // all parameters should be the same as in CustomerViewService.GetAccountHeaderViewModelAsync
            //        }).Result;

            //        var passwordChangedDateTime = customer.LastPasswordChanged.ToUniversalTime();
            //        // 10 seconds was added to compensate OCS server and EM server difference time.
            //        var ticketDateTime = (httpContext.User?.Identity as System.Web.Security.FormsIdentity)?.Ticket
            //            .IssueDate.ToUniversalTime().AddSeconds(10);
            //        if (passwordChangedDateTime > ticketDateTime)
            //        {
            //            RemoveComposerCookie(httpContext);
            //            RemoveAuth(httpContext);
            //            ReloadIfRequired(httpContext);
            //        }
            //    }
            //}


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
