using System;
using Composite.Data;
using Orckestra.Composer.CompositeC1.Pages;
using Orckestra.Composer.Search.Context;
using Orckestra.ExperienceManagement.Configuration.DataTypes;

namespace Orckestra.Composer.CompositeC1.Services
{
    public class CategoryMetaContext : ICategoryMetaContext
    {
        private IPageService PageService { get; }
        private Lazy<CategoryPage> CategoryPageLazy { get; }
        
        public CategoryMetaContext(IPageService pageService)
        {
            PageService = pageService;
            CategoryPageLazy = new Lazy<CategoryPage>(LoadCategoryPage);
        }

        public string GetCategoryId()
        {
            return CategoryPageLazy.Value?.CategoryId;
        }

        public bool GetIsAllProductPage()
        {
            return CategoryPageLazy.Value?.IsAllProductsPage ?? false;
        }

        private CategoryPage LoadCategoryPage()
        {
            var page = PageService.GetPage(SitemapNavigator.CurrentPageId);
            if (page == null)
                return null;

            var metaDefName = GetMetadataDefinitionName(page.PageTypeId);
            return page.GetMetaData(metaDefName, typeof(CategoryPage)) as CategoryPage;
        }

        private string GetMetadataDefinitionName(Guid pageTypeId)
        {
            var meta = pageTypeId == CategoryPages.CategoryLandingPageTypeId
                ? "ComposerCategoryLandingPage"
                : "ComposerCategoryPage";

            return meta;
        }
    };
}