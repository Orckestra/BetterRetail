using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.Pages;
using Orckestra.Composer.CompositeC1.Services;

namespace Orckestra.Composer.CompositeC1.Tests.Mocks
{
    public class PageServiceMock: IPageService
    {
        private readonly IEnumerable<IPage> _dataSource;

        public PageServiceMock(IEnumerable<IPage> dataSource)
        {
            _dataSource = dataSource;
        }

        public IPage GetPage(Guid pageId, CultureInfo cultureInfo = null)
        {
            return _dataSource.FirstOrDefault(p => p.Id == pageId);
        }

        public Guid GetParentPageId(IPage page)
        {
            var pageMock = page as PageMock;
            return (pageMock?.ParentPageId).GetValueOrDefault(Guid.Empty);
        }

        public string GetRendererPageUrl(Guid pageId, CultureInfo cultureInfo = null)
        {
            throw new NotImplementedException();
        }

        public string GetPageUrl(Guid pageId, CultureInfo cultureInfo = null)
        {
            var page = _dataSource.FirstOrDefault(p => p.Id == pageId);

            return page?.FriendlyUrl;
        }

        public string GetPageUrl(IPage page)
        {
            var pageMock = page as PageMock;
            return pageMock?.Url;
        }

        public List<CheckoutStepInfoPage> GetCheckoutStepPages(CultureInfo cultureInfo = null)
        {
            throw new NotImplementedException();
        }

        public CheckoutStepInfoPage GetCheckoutStepPage(Guid pageId, CultureInfo cultureInfo = null)
        {
            throw new NotImplementedException();
        }
    }
}
