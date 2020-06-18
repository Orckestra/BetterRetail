using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public BreadcrumbViewService(IPageService pageService, ISiteConfiguration siteConfiguration)
        {
            _pageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            SiteConfiguration = siteConfiguration;
        }

        public virtual BreadcrumbViewModel CreateBreadcrumbViewModel(GetBreadcrumbParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }

            var pageId = new Guid(param.CurrentPageId);
            var page = _pageService.GetPageNode(pageId, param.CultureInfo) ?? 
                throw new InvalidOperationException("Could not find any page matching this ID.");

            var breadcrumbViewModel = new BreadcrumbViewModel
            {
                ActivePageName = page.MenuTitle,
                Items = CreateBreadcrumbItems(param, page)
            };

            return breadcrumbViewModel;
        }

        protected virtual List<BreadcrumbItemViewModel> CreateBreadcrumbItems(GetBreadcrumbParam param, PageNode page)
        {
            var breadcrumbStack = new Stack<BreadcrumbItemViewModel>();
            var parentPageId = PageManager.GetParentId(page.Id);
            //var parentPageId = _pageService.GetParentPageId(page);

            while (parentPageId != Guid.Empty)
            {
                var parentPage = _pageService.GetPageNode(parentPageId, param.CultureInfo);

                var itemVM = CreateParentPageItem(parentPage);
                breadcrumbStack.Push(itemVM);

                parentPageId = PageManager.GetParentId(parentPage.Id);
                //parentPageId = _pageService.GetParentPageId(parentPage);
            }

            var items = UnrollStack(breadcrumbStack).ToList();
            return items;
        }

        protected virtual BreadcrumbItemViewModel CreateParentPageItem(PageNode parentPage)
        {
            var itemVM = new BreadcrumbItemViewModel
            {
                DisplayName = parentPage.MenuTitle
            };

            var pagesConfiguration = SiteConfiguration.GetPagesConfiguration();
            if (pagesConfiguration!= null && parentPage.GetPageProperty(p => p.PageTypeId) != pagesConfiguration.FolderId)
            {
                //itemVM.Url = _pageService.GetPageUrl(parentPage);
                itemVM.Url = parentPage.Url;
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
    internal static class PageNodeExtensions
    {
        public static T GetPageProperty<T>(this PageNode pageNode, Func<IPage, T> getPropertyMethod)
        {
            var pageProp = typeof(PageNode).GetProperty("_page", BindingFlags.NonPublic | BindingFlags.Instance);
            var getter = pageProp?.GetGetMethod(nonPublic: true);
            return getter?.Invoke(pageNode, null) is IPage page ? getPropertyMethod(page) : default;
        }

    }
}