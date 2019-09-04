using System;
using System.Collections.Generic;
using System.Globalization;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.Pages;

namespace Orckestra.Composer.CompositeC1.Services
{
    public interface IPageService
    {
        Guid GetCurrentHomePageId();
        IPage GetPage(Guid pageId, CultureInfo cultureInfo = null);

        Guid GetParentPageId(IPage page);

        string GetRendererPageUrl(Guid pageId, CultureInfo cultureInfo = null);

        string GetPageUrl(Guid pageId, CultureInfo cultureInfo = null);

        string GetPageUrl(IPage page);

        List<CheckoutStepInfoPage> GetCheckoutStepPages(CultureInfo cultureInfo = null);

        CheckoutStepInfoPage GetCheckoutStepPage(Guid pageId, CultureInfo cultureInfo = null);
    }
}
