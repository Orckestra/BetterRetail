using Composite.Data;
using Composite.Data.Types;
using Orckestra.Composer.CompositeC1.DataTypes.Navigation;
using Orckestra.Composer.CompositeC1.Providers.MainMenu;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.CompositeC1.Services.Cache;
using Orckestra.Composer.Recipes.DataTypes;
using Orckestra.Composer.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orckestra.Composer.Recipes
{
    public class RecipesMainMenuItemsProvider : IMainMenuItemsProvider
    {
        protected ICacheStore<Guid, IEnumerable<MainMenuItemWrapper>> Cache { get; }
        public IComposerContext ComposerContext { get; set; }

        protected IPageService PageService { get; }

        protected IRecipeUrlProvider RecipeUrlProvider { get; }

        public RecipesMainMenuItemsProvider(
           IComposerContext composerContext,
           IPageService pageService,
           IRecipeUrlProvider recipeUrlProvider,
           ICacheService cacheService)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));
            RecipeUrlProvider = recipeUrlProvider ?? throw new ArgumentNullException(nameof(recipeUrlProvider));
            Cache = cacheService.GetStoreWithDependencies<Guid, IEnumerable<MainMenuItemWrapper>>("Recipes Menu Items",
               new CacheDependentEntry<IRecipe>(),
               new CacheDependentEntry<IRecipeMealType>(),
               new CacheDependentEntry<IMainMenuProviderConfiguration>()
           );
        }

        public IEnumerable<MainMenuItemWrapper> GetMainMenuItems(Guid websiteId)
        {
            return Cache.GetOrAdd(websiteId, _ => LoadMenuItems(websiteId));
        }

        private IEnumerable<MainMenuItemWrapper> LoadMenuItems(Guid websiteId)
        {
            var recipeMealTypes = DataFacade.GetData<IRecipeMealType>().ToList();
            var configuration = DataFacade.GetData<IMainMenuProviderConfiguration>().FirstOrDefault(i => i.PageId == websiteId && i.Provider == typeof(RecipesMainMenuItemsProvider).Name);

            if (configuration == null) return null;

            Guid? parentId = null;
            MainMenuItemWrapper root = null;

            if (configuration.PageId != null)
            {
                var page = DataFacade.GetData<IPage>().FirstOrDefault(p => p.Id == configuration.ParentPage);
                if (page != null)
                {
                    root = new MainMenuItemWrapper()
                    {
                        Id = Guid.NewGuid(),
                        DisplayName = page.MenuTitle,
                        Url = PageService.GetPageUrl(page.Id, ComposerContext.CultureInfo),
                        SourceCultureName = page.SourceCultureName,
                        CssClassName = string.Empty,
                        Order = configuration.Order
                    };

                    parentId = root.Id;
                }
            }

            var recipes = DataFacade.GetData<IRecipe>();
            var items = recipeMealTypes.Where(t=> recipes.Any(r=> r.MealType == t.Id)).Select(item => new MainMenuItemWrapper()
            {
                Id = Guid.NewGuid(),
                DisplayName = item.Title,
                Url = RecipeUrlProvider.BuildRecipeMealTypeUrl(item.Id, ComposerContext.CultureInfo),
                SourceCultureName = item.SourceCultureName,
                CssClassName = string.Empty,
                Order = item.Order,
                ParentId = parentId
            }).ToList();

            if (root != null)
            {
                items.Add(root);
            }

            return items;
        }

        public bool IsActive(Guid websiteId)
        {
            return DataFacade.GetData<IMainMenuProviderConfiguration>().FirstOrDefault(i => i.PageId == websiteId && i.Provider == typeof(RecipesMainMenuItemsProvider).Name) != null;
        }
    }
}
