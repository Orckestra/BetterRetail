using System.Web.Mvc;
using Composite.Core.Routing.Pages;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1
{
    public class AuthorizeAndRedirectAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var pageService = new PageService();
            string loginUrl = string.Empty;

            if (C1PageRoute.PageUrlData != null)
            {
                var culture = C1PageRoute.PageUrlData.LocalizationScope;
                loginUrl = pageService.GetPageUrl(PagesConfiguration.LoginPageId, culture);
            }
            else
            {
                loginUrl = pageService.GetPageUrl(PagesConfiguration.LoginPageId);
            }
            filterContext.Result = new RedirectResult(loginUrl);

        }
    }
}
