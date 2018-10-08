using System;
using System.Web;
using Composite.C1Console.Security;
using Orckestra.Composer.HttpModules;

namespace Orckestra.Composer.CompositeC1
{
    public class AntiCookieTamperingExcluder : IAntiCookieTamperingExcluder
    {
        public bool ShouldHandleRequest(HttpContextBase httpContext)
        {
            return !UserValidationFacade.IsLoggedIn();
        }
    }
}
