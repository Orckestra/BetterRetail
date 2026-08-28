using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Composite.Data;
using Composite.Data.Types;
using Orckestra.Composer.Services.Breadcrumb;
using Orckestra.Composer.ViewModels.Breadcrumb;
using Orckestra.ExperienceManagement.Configuration;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class BreadcrumbViewService : IBreadcrumbViewService
    {
        protected IPageService _pageService;
        protected ISiteConfiguration SiteConfiguration { get; private set; }
        protected IDataConnectionService DataConnectionService { get; private set; }

        public BreadcrumbViewService(IPageService pageService, ISiteConfiguration siteConfiguration, IDataConnectionService dataConnectionService)
        {
            _pageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            SiteConfiguration = siteConfiguration;
            DataConnectionService = dataConnectionService;
        }

        public virtual BreadcrumbViewModel CreateBreadcrumbViewModel(GetBreadcrumbParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            var pageId = new Guid(param.CurrentPageId);
            using (var connection = DataConnectionService.GetDataConnection(param.CultureInfo))
            {
                var page = connection.GetPageNodeById(pageId) ?? throw new InvalidOperationException("Could not find any page matching this ID."); ;
                var breadcrumbViewModel = new BreadcrumbViewModel
                {
                    ActivePageName = HttpUtility.HtmlEncode(page.MenuTitle),
                    Items = CreateBreadcrumbItems(param, page, connection)
                };
                return breadcrumbViewModel;
            }
        }

        protected virtual List<BreadcrumbItemViewModel> CreateBreadcrumbItems(GetBreadcrumbParam param, PageNode page, IDataConnectionAdapter connection)
        {
            var breadcrumbStack = new Stack<BreadcrumbItemViewModel>();
            var parentPageId = page.ParentPage?.Id ?? Guid.Empty;

            while (parentPageId != Guid.Empty)
            {
                var parentPage = connection.GetPageNodeById(parentPageId);
                if (parentPage != null)
                {
                    var itemVM = CreateBreadcrumbItemViewModel(parentPage);
                    breadcrumbStack.Push(itemVM);
                    parentPageId = parentPage.ParentPage?.Id ?? Guid.Empty;
                }
                else
                {
                    parentPageId = Guid.Empty;
                }
            }

            var items = UnrollStack(breadcrumbStack).ToList();
            return items;
        }

        protected virtual BreadcrumbItemViewModel CreateBreadcrumbItemViewModel(PageNode page)
        {
            var itemVM = new BreadcrumbItemViewModel
            {
                DisplayName = page.MenuTitle
            };

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration();
            if (pagesConfiguration != null && page.Page.PageTypeId != pagesConfiguration.FolderId)
            {
                itemVM.Url = page.Url;
            }

            return itemVM;
        }

        protected virtual IEnumerable<BreadcrumbItemViewModel> UnrollStack(Stack<BreadcrumbItemViewModel> breadcrumbStack)
        {
            while (breadcrumbStack.Count > 0)
            {
                yield return breadcrumbStack.Pop();
            }
        }
    }
}