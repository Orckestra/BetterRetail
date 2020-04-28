using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Composite.Data.Types;

namespace Orckestra.Composer.CompositeC1.Services
{
    public interface IPageService
    {
        IPage GetPage(Guid pageId, CultureInfo cultureInfo = null);

        Guid GetParentPageId(IPage page);

        string GetRendererPageUrl(Guid pageId, CultureInfo cultureInfo = null);

        string GetPageUrl(Guid pageId, CultureInfo cultureInfo = null, HttpContext httpContext = null);

        string GetPageUrl(IPage page);
   
    }
}
