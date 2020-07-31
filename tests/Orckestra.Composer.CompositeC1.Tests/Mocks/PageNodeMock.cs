using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Composite.Data;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.Services;

namespace Orckestra.Composer.CompositeC1.Tests.Mocks
{
    public class PageNodeMock : PageNode
    {
        private string _url;
        public IPage Page { get; }
        public IPageService PageService { get; }

        public PageNodeMock(IPage page, IPageService pageService) : base(page, null)
        {
            Page = page;
            PageService = pageService;
        }

        public override PageNode ParentPage
        {
            get
            {
                var parentPageId = PageService.GetParentPageId(Page);
                if (parentPageId != Guid.Empty)
                {
                    return PageService.GetPageNode(parentPageId);
                }

                return null;
            }
        }

        public override string Url => (Page as PageMock)?.Url;
    }
}
