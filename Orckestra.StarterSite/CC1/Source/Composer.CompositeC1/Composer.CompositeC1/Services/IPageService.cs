using System;
using System.Collections.Generic;
using System.Globalization;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.Pages;

namespace Orckestra.Composer.CompositeC1.Services
{
    public interface IPageService
    {
        IPage GetPage(Guid pageId, CultureInfo cultureInfo = null);

        Guid GetParentPageId(IPage page);

        string GetRendererPageUrl(Guid pageId, CultureInfo cultureInfo = null);

        string GetPageUrl(Guid pageId, CultureInfo cultureInfo = null);

        string GetPageUrl(IPage page);

        List<string> GetCheckoutStepPages(Guid currentHomePageId, CultureInfo cultureInfo = null);
        List<string> GetCheckoutNavigationPages(Guid currentHomePageId, CultureInfo cultureInfo = null);
        int GetCheckoutStepPageNumber(Guid currentHomePageId, Guid pageId, CultureInfo cultureInfo = null);
    }
}
