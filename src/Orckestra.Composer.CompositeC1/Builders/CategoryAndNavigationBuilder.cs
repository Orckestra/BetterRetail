using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Composite.Core;
using Composite.Data;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.DataTypes.Navigation;
using Orckestra.Composer.CompositeC1.Pages;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Utils;
using Orckestra.ExperienceManagement.Configuration;
using Orckestra.ExperienceManagement.Configuration.DataTypes;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.CompositeC1.Builders
{

    public static class NavigationNamespaces
    {
        public static readonly Guid MainMenuNamespaceId = new Guid("53EE196F-43E9-4584-B161-FB13E461448F");
    }

    public class CategoryAndNavigationBuilder : ICategoryAndNavigationBuilder
    {
        public ICategoryRepository Repository { get; }


        public CategoryAndNavigationBuilder(ICategoryRepository repository)
        {
            Repository = repository;
        }


        // TODO: remove when ICategoryPageService wil be moved to common library
        private ICollection<Guid> CategoryPageService_CreateCategoryPages(Guid websiteId,
            IEnumerable<string> categoryIds, CultureInfo cultureInfo, Guid pageTypeId, bool publishPages)
        {
            var categoryPageServiceType =
                Type.GetType(
                    "Orckestra.ExperienceManagement.CategoryMerchandising.Services.ICategoryPageSevice, Orckestra.ExperienceManagement.CategoryMerchandising");


            var categoryPageService = ServiceLocator.GetService(categoryPageServiceType);

            var methodInfo = categoryPageServiceType?.GetMethod("CreateCategoryPages",
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            return methodInfo?.Invoke(categoryPageService,
                new object[] { websiteId, categoryIds, cultureInfo, pageTypeId, publishPages }) as ICollection<Guid>;
        }


        public void ReBuildCategoriesAndMenu(Dictionary<string, string> localizedDisplayNames)
        {
            foreach (var culture in DataLocalizationFacade.ActiveLocalizationCultures)
            {
                using (var data = new DataConnection(PublicationScope.Unpublished, culture))
                {
                    var currentPages = from p in DataFacade.GetData<IPage>()
                                       join catPAge in DataFacade.GetData<CategoryPage>() on p.Id equals catPAge.PageId
                                       select p;

                    DataFacade.Delete(currentPages.ToList(), CascadeDeleteType.Allow);

                    DataFacade.Delete<NavigationImage>(d => true);

                    Func<List<MainMenu>> getMainMenuWithoutRefs = () =>
                        DataFacade.GetData<MainMenu>().Where(d => d.GetReferees(typeof(MainMenu), null, false).Count == 0).ToList();

                    var mainMenuWithoutRefs = getMainMenuWithoutRefs();
                    // sometimes optional refs is broken and updating optional link throw error on republish
                    while (mainMenuWithoutRefs.Any())
                    {
                        mainMenuWithoutRefs.ForEach(d =>
                        {
                            DataFacade.Delete(d, true);
                        });
                        mainMenuWithoutRefs = getMainMenuWithoutRefs();
                    }

                    DataFacade.Delete<MainMenu>(d => true);

                    foreach (var websiteId in data.SitemapNavigator.HomePageIds)
                    {
                        string scope = null;
                        try
                        {
                            scope = DataFacade.GetData<ISiteConfigurationMeta>(d => d.PageId == websiteId).Select(d => d.Scope).FirstOrDefault();
                        }
                        catch
                        {
                            // ignored
                        }

                        if (string.IsNullOrWhiteSpace(scope)) continue;

                        var pageTypeId = CategoryPages.CategoryPageTypeId;


                        if (string.IsNullOrWhiteSpace(scope)) continue;

                        List<Category> categories = Repository.GetCategoriesAsync(new GetCategoriesParam()
                        {
                            Scope = scope
                        }).Result;

                        var selectedKeys = categories.Select(d => d.Id).ToList();

                        var newPageIds = CategoryPageService_CreateCategoryPages(
                            websiteId, selectedKeys, culture, pageTypeId, false).ToList();

                        newPageIds.ForEach(pageId =>
                        {
                            var page = PageManager.GetPageById(pageId);
                            page.PublicationStatus = GenericPublishProcessController.Published;
                            DataFacade.Update(page);
                        });

                        Dictionary<string, Guid> catPageIds = (from p in newPageIds
                                                               join cp in DataFacade.GetData<CategoryPage>() on p equals cp.PageId
                                                               select new { cp.PageId, cp.CategoryId }).ToDictionary(d => d.CategoryId, d => d.PageId);


                        var mainMenu = DataFacade.BuildNew<MainMenu>();
                        mainMenu.Id = GuidUtility.Create(NavigationNamespaces.MainMenuNamespaceId, "Root");
                        mainMenu.PageId = websiteId;
                        mainMenu.DisplayName = (localizedDisplayNames?.ContainsKey(culture.Name) ?? false) ? localizedDisplayNames[culture.Name] : string.Empty;
                        mainMenu.CssClassName = string.Empty;
                        mainMenu.ParentId = null;
                        mainMenu.Url = $"~/page({websiteId})";
                        mainMenu.PublicationStatus = GenericPublishProcessController.Published;
                        mainMenu = DataFacade.AddNew(mainMenu);

                        new MenuBuilder(categories, catPageIds, culture.Name, websiteId).CreateMainMenu("Root", mainMenu.Id, 2);

                    }

                }
            }
        }


        internal class MenuBuilder
        {
            public MenuBuilder(IReadOnlyCollection<Category> categories, IReadOnlyDictionary<string, Guid> pageIds, string culture, Guid websiteId)
            {

                Categories = categories;
                PageIds = pageIds;
                Culture = culture;
                WebsiteId = websiteId;
            }

            public IReadOnlyCollection<Category> Categories { get; private set; }

            public IReadOnlyDictionary<string, Guid> PageIds { get; private set; }

            public string Culture { get; private set; }
            public Guid WebsiteId { get; private set; }


            public void CreateMainMenu(string primaryParentCategoryId, Guid? parentMainMenuId, int depth)
            {
                if (depth == 0)
                    return;
                Categories.Where(d => d.PrimaryParentCategoryId == primaryParentCategoryId).ToList().ForEach(cat =>
                {
                    Guid pageId;
                    if (PageIds.TryGetValue(cat.Id, out pageId))
                    {
                        var mainMenu = DataFacade.BuildNew<MainMenu>();
                        mainMenu.Id = GuidUtility.Create(NavigationNamespaces.MainMenuNamespaceId, cat.Id);
                        mainMenu.PageId = WebsiteId;
                        mainMenu.DisplayName = cat.DisplayName.GetLocalizedValue(Culture);
                        mainMenu.CssClassName = string.Empty;
                        mainMenu.ParentId = parentMainMenuId;
                        mainMenu.Url = $"~/page({pageId})";
                        mainMenu.PublicationStatus = GenericPublishProcessController.Published;
                        mainMenu = DataFacade.AddNew(mainMenu);

                        CreateMainMenu(cat.Id, mainMenu.Id, depth - 1);

                    }
                });
            }
        }
    }
}