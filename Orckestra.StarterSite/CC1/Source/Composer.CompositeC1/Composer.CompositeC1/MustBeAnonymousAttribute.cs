using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Composite.Data;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Utils;

namespace Orckestra.Composer.CompositeC1
{
    /// <summary>
    /// Method accessible only if user is not logged in. Otherwise redirect to destination specified in parameter.
    /// </summary>
    public class MustBeAnonymousAttribute : ActionFilterAttribute
    {
        public const string MyAccountDestination = "MyAccount";
        public const string CartDestination = "Cart";

        protected Func<HttpContextBase, string> GetRedirectUrl { get; private set; } 

        public MustBeAnonymousAttribute(string destination)
        {
            GetRedirectUrl = GetRedirectUrlMethod(destination);
        }

        private Func<HttpContextBase, string> GetRedirectUrlMethod(string destination)
        {
            switch (destination)
            {
                case MyAccountDestination:
                    return GetRedirectUrlForMyAccount;

                case CartDestination:
                    return GetRedirectUrlForCart;

                default:
                    return httpContext => "/";
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (IsUserAuthenticated(filterContext.HttpContext))
            {
                var url = GetRedirectUrl(filterContext.HttpContext);
                url = UrlFormatter.AppendQueryString(url, new NameValueCollection
                {
                    {"logged", "true"}
                });

                filterContext.Redirect(url);
            }

            base.OnActionExecuting(filterContext);
        }

        private string GetRedirectUrlForMyAccount(HttpContextBase httpContext)
        {
            var urlProvider = ComposerHost.Current.Resolve<IMyAccountUrlProvider>();

            var url = urlProvider.GetMyAccountUrl(new BaseUrlParameter
            {
                CultureInfo = CultureInfo.CurrentCulture
            });
            return url;
        }

        private string GetRedirectUrlForCart(HttpContextBase httpContext)
        {
            var urlProvider = ComposerHost.Current.Resolve<ICartUrlProvider>();

            var url = urlProvider.GetCartUrl(new BaseUrlParameter
            {                
                CultureInfo = CultureInfo.CurrentCulture
            });
            return url;
        }

        private static bool IsUserAuthenticated(HttpContextBase httpContext)
        {
            bool IsAuthenticated = httpContext.User?.Identity.IsAuthenticated ?? false;
            bool IsCurrentWebSite = (httpContext.User?.Identity as System.Web.Security.FormsIdentity)?.Ticket.UserData == SitemapNavigator.CurrentHomePageId.ToString();
            return IsAuthenticated && IsCurrentWebSite;
        }
    }
}
