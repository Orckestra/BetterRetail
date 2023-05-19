using Composite.Core.Implementation;
using Composite.Data;
using Composite.Data.Types;
using Moq;
using Orckestra.Composer.CompositeC1.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Orckestra.Composer.CompositeC1.Tests.Mocks
{
    public class DataConnectionServiceMock : IDataConnectionService
    {
        private readonly IEnumerable<IPage> _pagesSource;
        public DataConnectionServiceMock(IEnumerable<IPage> pagesSource)
        {
            _pagesSource = pagesSource;
        }

        public IDataConnectionAdapter GetDataConnection(CultureInfo culture)
        {
            var mockDC = new Mock<IDataConnectionAdapter>();
            var navigator = new Mock<SitemapNavigatorImplementation>();
            mockDC.Setup(sm => sm.GetPageNodeById(It.IsAny<Guid>())).Returns<Guid>((id) =>
            {
                var page = _pagesSource.FirstOrDefault(p => p.Id == id) as PageMock;
                if (page == null) return null;
                var parentPage = _pagesSource.FirstOrDefault(p => page.ParentPageId == p.Id) as PageMock;
                var parentPageNode = parentPage != null ? new PageNode(parentPage, navigator.Object) : null;
                var pageNodeMock = new Mock<PageNode>(page, navigator.Object);
                pageNodeMock.SetupGet(p => p.ParentPage).Returns(parentPageNode);
                pageNodeMock.SetupGet(p => p.Page).Returns(page);
                pageNodeMock.SetupGet(p => p.MenuTitle).Returns(page.MenuTitle);
                pageNodeMock.SetupGet(p => p.Url).Returns(page.Url);
                return pageNodeMock.Object;
            });
            return mockDC.Object;
        }
    }
}
