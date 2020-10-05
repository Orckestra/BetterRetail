using System.Web;
using Composite.Core;
using Orckestra.Composer.CompositeC1;
using Orckestra.Composer.Services;
using Orckestra.Composer.Services.Cookie;

namespace Orckestra.Composer.Grocery
{
    public class GroceryHttpApplication: C1HttpApplication
    {
        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            var cacheKey = base.GetVaryByCustomString(context, custom);

            var cookieAccessor = ServiceLocator.GetService<ICookieAccessor<ComposerCookieDto>>();
            var extendedCookieInfo = new ExtendedCookieData(cookieAccessor.Read());

            if (!string.IsNullOrEmpty(extendedCookieInfo.SelectedStoreNumber))
            {
                cacheKey += extendedCookieInfo.SelectedStoreNumber;
            }
            else if (extendedCookieInfo.BrowseWithoutStore)
            {
                cacheKey += "BrowseWithoutStore";
            }

            return cacheKey;
        }
    }
}
