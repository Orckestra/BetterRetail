using System;
using System.Web;
using Composite.Data.Types;

namespace Orckestra.Composer.CompositeC1.Utils
{
    public static class HttpContextHelper
    {
        public static Guid? GetCurrentPageId(this HttpContextBase httpContext)
        {
            const string pageIdKey = "PageRenderer.IPage";
            if (httpContext == null 
                || !httpContext.Items.Contains(pageIdKey))
                return null;

            var page = httpContext.Items[pageIdKey] as IPage;

            return page?.Id;
        }
    };
}
