using Composite.Core;
using Composite.Core.Routing.Pages;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.ExperienceManagement.Configuration;
using System.Web.Mvc;

namespace Orckestra.Composer.CompositeC1
{
    public class AuthorizeAndRedirectAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var siteConfiguration = ServiceLocator.GetService<ISiteConfiguration>();
            var pageService = new PageService();
            var loginPageId = siteConfiguration.GetPagesConfiguration().LoginPageId;
            string loginUrl = string.Empty;

            if (C1PageRoute.PageUrlData != null)
            {
                var culture = C1PageRoute.PageUrlData.LocalizationScope;
                loginUrl = pageService.GetPageUrl(loginPageId, culture);
            }
            else
            {
                loginUrl = pageService.GetPageUrl(loginPageId);
            }
            filterContext.Result = new RedirectResult(loginUrl);

        }
    }
}
