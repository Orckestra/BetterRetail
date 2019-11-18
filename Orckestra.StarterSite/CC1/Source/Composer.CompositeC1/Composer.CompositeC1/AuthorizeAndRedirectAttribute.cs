using Composite.Core;
using Composite.Core.Routing.Pages;
using Composite.Data;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.ExperienceManagement.Configuration;
using System.Web;
using System.Web.Mvc;

namespace Orckestra.Composer.CompositeC1
{
    public class AuthorizeAndRedirectAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var websiteId = SitemapNavigator.CurrentHomePageId;
            var userData = (httpContext.User.Identity as System.Web.Security.FormsIdentity)?.Ticket.UserData;

            return base.AuthorizeCore(httpContext) && userData == websiteId.ToString();
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var siteConfiguration = ServiceLocator.GetService<ISiteConfiguration>();
            var pageService = ServiceLocator.GetService<IPageService>();
            var loginPageId = siteConfiguration.GetPagesConfiguration().LoginPageId;
            var culture = C1PageRoute.PageUrlData?.LocalizationScope;
            string loginUrl = pageService.GetPageUrl(loginPageId, culture);

            filterContext.Result = new RedirectResult(loginUrl);
        }
    }
}