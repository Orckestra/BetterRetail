using Orckestra.Composer.CompositeC1.Providers.MainMenu;
using Orckestra.Composer.CompositeC1.Services;
using Orckestra.Composer.CompositeC1.Services.Cache;
using Orckestra.Composer.Services;
using System;
using System.Collections.Generic;
using Composite.Core.Routing;
using Composite.Data;
using System.Linq;
using Orckestra.Composer.CompositeC1.DataTypes.Navigation;
using Composite.Data.Types;

namespace Orckestra.Composer.Grocery.Providers
{
    public class MyUsualsMainMenuItemsProvider : IMainMenuItemsProvider
    {
        protected ICacheStore<string, IEnumerable<MainMenuItemWrapper>> Cache { get; }
        public IComposerContext ComposerContext { get; set; }
        protected IPageService PageService { get; }

        public MyUsualsMainMenuItemsProvider(
           IComposerContext composerContext,
           IPageService pageService,
           ICacheService cacheService)
        {
            ComposerContext = composerContext ?? throw new ArgumentNullException(nameof(composerContext));
            PageService = pageService ?? throw new ArgumentNullException(nameof(pageService));

            Cache = cacheService.GetStoreWithDependencies<string, IEnumerable<MainMenuItemWrapper>>("Grocery Menu Items",
                 new CacheDependentEntry<IMainMenuProviderConfiguration>());
        }

        public IEnumerable<MainMenuItemWrapper> GetMainMenuItems(Guid websiteId)
        {
            var key = $"{websiteId}.{ComposerContext.CultureInfo}.{new UrlSpace().ForceRelativeUrls}";
            return Cache.GetOrAdd(key, _ => LoadMenuItems(websiteId));
        }

        private IEnumerable<MainMenuItemWrapper> LoadMenuItems(Guid websiteId)
        {
            var configuration = DataFacade.GetData<IMainMenuProviderConfiguration>().FirstOrDefault(i => i.PageId == websiteId && i.Provider == typeof(MyUsualsMainMenuItemsProvider).Name);

            if (configuration == null) return null;

            if (configuration.ParentPage != null)
            {
                var page = DataFacade.GetData<IPage>().FirstOrDefault(p => p.Id == configuration.ParentPage);
                if (page != null)
                {

                    return new MainMenuItemWrapper[]
                    {
                         new MainMenuItemWrapper()
                            {
                                Id = Guid.NewGuid(),
                                DisplayName = page.MenuTitle,
                                Url = PageService.GetPageUrl(page.Id, ComposerContext.CultureInfo),
                                SourceCultureName = page.SourceCultureName,
                                CssClassName = "only-authorized",
                                Order = configuration.Order
                            }
                    };
                }
            }

            return new MainMenuItemWrapper[] {};
        }

        public bool IsActive(Guid websiteId)
        {
            return DataFacade.GetData<IMainMenuProviderConfiguration>().FirstOrDefault(i => i.PageId == websiteId && i.Provider == typeof(MyUsualsMainMenuItemsProvider).Name) != null;
        }
    }
}
