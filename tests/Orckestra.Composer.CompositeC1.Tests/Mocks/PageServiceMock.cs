using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Composite.Data;

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
            throw new NotImplementedException();
        }

        PageNode IPageService.GetPageNode(Guid pageId, CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }

        public IPage GetPageNode(Guid pageId, CultureInfo cultureInfo = null)
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

        public string GetPageUrl(Guid pageId, CultureInfo cultureInfo = null, HttpContext httpContext = null)
        {
            var page = _dataSource.FirstOrDefault(p => p.Id == pageId);

            return page?.FriendlyUrl;
        }

        public string GetPageUrl(IPage page)
        {
            var pageMock = page as PageMock;
            return pageMock?.Url;
        }

        public List<string> GetCheckoutStepPages(Guid homepageId, CultureInfo cultureInfo = null)
        {
            throw new NotImplementedException();
        }

        public List<string> GetCheckoutNavigationPages(Guid homepageId, CultureInfo cultureInfo = null)
        {
            throw new NotImplementedException();
        }

        public int GetCheckoutStepPageNumber(Guid currentHomePageId, Guid pageId, CultureInfo cultureInfo = null)
        {
            throw new NotImplementedException();
        }
    }
}
