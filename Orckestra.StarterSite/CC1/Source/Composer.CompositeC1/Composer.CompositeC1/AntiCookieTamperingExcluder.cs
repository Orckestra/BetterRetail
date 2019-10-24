using System;
using System.Web;
using Composite.C1Console.Security;
using Orckestra.Composer.HttpModules;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1
{
    public class AntiCookieTamperingExcluder : IAntiCookieTamperingExcluder
    {
        private IWebsiteContext WebsiteContext { get; set; }
        public AntiCookieTamperingExcluder(IWebsiteContext websiteContext)
        {
            WebsiteContext = websiteContext;
        }

        public bool ShouldHandleRequest(HttpContextBase httpContext)
        {
            return !UserValidationFacade.IsLoggedIn() && WebsiteContext.WebsiteId != Guid.Empty;
        }
    }
}
