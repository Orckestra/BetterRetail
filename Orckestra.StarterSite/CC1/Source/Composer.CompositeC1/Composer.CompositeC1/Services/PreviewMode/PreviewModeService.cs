using System;
using System.Linq;
using System.Web;
using Orckestra.Composer.CompositeC1.DataTypes;
using Orckestra.Composer.CompositeC1.Services.Cache;
using Orckestra.Composer.CompositeC1.Services.DataQuery;
using Orckestra.Composer.CompositeC1.Utils;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Composer.Services;

namespace Orckestra.Composer.CompositeC1.Services.PreviewMode
{
    public class PreviewModeService : IPreviewModeService
    {
        protected ICacheStore<Guid, string> Cache { get; }
        protected IDataQueryService DataQueryService { get; }
        protected IProductRepository ProductRepository { get; }
        protected ISearchRepository SearchRepository { get; }
        protected HttpContextBase HttpContext { get; }
        protected IComposerContext ComposerContext { get; }

        public PreviewModeService(HttpContextBase httpContext, ICacheService cacheService, IDataQueryService dataQueryService,
            IProductRepository productRepository, ISearchRepository searchRepository, IComposerContext composerContext)
        {
            HttpContext = httpContext;
            DataQueryService = dataQueryService;
            ProductRepository = productRepository;
            SearchRepository = searchRepository;
            ComposerContext = composerContext;
            Cache = cacheService.GetStoreWithDependencies<Guid, string>("Preview Mode", new CacheDependentEntry<IPreviewModeMeta>());
        }

        public virtual string GetProductId()
        {
            var pageId = HttpContext.GetCurrentPageId();
            return Cache.GetOrAdd(pageId.GetValueOrDefault(), _ => LoadProductId(pageId));
        }

        protected virtual string LoadProductId(Guid? pageId)
        {
            string productId = null;
            
            if (pageId.HasValue)
                productId = LoadProductIdFromPage(pageId.Value);

            if (VerifyProductIdValid(productId))
                return productId;

            var products = SearchRepository.SearchProductAsync(new SearchCriteria
            {
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                NumberOfItemsPerPage = 1,
                Page = 1,
            }).GetAwaiter().GetResult();

            return products?.Documents?.FirstOrDefault()?.ProductId;
        }

        protected virtual string LoadProductIdFromPage(Guid pageId)
        {
            return DataQueryService
                .Get<IPreviewModeMeta>()
                .Where(x => x.PageId == pageId)
                .Select(x => x.ProductId)
                .FirstOrDefault();
        }

        protected virtual bool VerifyProductIdValid(string productId)
        {
            if (string.IsNullOrEmpty(productId))
                return false;

            var product = ProductRepository.GetProductAsync(new GetProductParam
            {
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                ProductId = productId,
            }).GetAwaiter().GetResult();

            return product != null;
        }
    };
}
