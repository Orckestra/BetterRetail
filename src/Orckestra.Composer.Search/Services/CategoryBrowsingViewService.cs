using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Factory;
using Orckestra.Composer.Search.Helpers;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Providers;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Products;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Search.Services
{
    /// <summary>
    /// Service use to retrieves the products from the browsing pages.
    /// </summary>
    public class CategoryBrowsingViewService : BaseSearchViewService<BrowsingSearchParam>, ICategoryBrowsingViewService
    {
        public override SearchType SearchType => SearchType.Browsing;

        protected ICategoryRepository CategoryRepository { get; }
        protected ICategoryBrowsingUrlProvider CategoryBrowsingUrlProvider { get; }

        public CategoryBrowsingViewService(
            ISearchRepository searchRepository,
            IViewModelMapper viewModelMapper,
            IDamProvider damProvider,
            ILocalizationProvider localizationProvider,
            IProductUrlProvider productUrlProvider,
            ISearchUrlProvider searchUrlProvider,
            ICategoryRepository categoryRepository,
            ICategoryBrowsingUrlProvider categoryBrowsingUrlProvider,
            IFacetFactory facetFactory,
            ISelectedFacetFactory selectedFacetFactory,
            IPriceProvider priceProvider,
            IComposerContext composerContext,
            IProductSettingsViewService productSettings,
            IScopeViewService scopeViewService,
            IRecurringOrdersSettings recurringOrdersSettings)

            : base(
            searchRepository,
            viewModelMapper,
            damProvider,
            localizationProvider,
            productUrlProvider,
            searchUrlProvider,
            facetFactory,
            selectedFacetFactory,
            priceProvider,
            composerContext,
            productSettings,
            scopeViewService,
            recurringOrdersSettings)
        {
            CategoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            CategoryBrowsingUrlProvider = categoryBrowsingUrlProvider ?? throw new ArgumentNullException(nameof(categoryBrowsingUrlProvider));
        }

        public virtual async Task<CategoryBrowsingViewModel> GetCategoryBrowsingViewModelAsync(GetCategoryBrowsingViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (param.CategoryId == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.CategoryId)), nameof(param)); }
            if (param.SelectedFacets == null) { throw new ArgumentException(GetMessageOfNull(nameof(param.SelectedFacets)), nameof(param)); }

            var node = await GetCurrentCategoryNodeAsync(param).ConfigureAwait(false);
            var landingPageUrls = GetLandingPageUrls(node, param);

            var viewModel = new CategoryBrowsingViewModel
            {
                CategoryId = param.CategoryId,
                CategoryName = param.CategoryName,
                SelectedFacets = await GetSelectedFacetsAsync(param).ConfigureAwait(false),
                ProductSearchResults = await GetProductSearchResultsAsync(param).ConfigureAwait(false),
                ChildCategories = await GetChildCategoriesAsync(param).ConfigureAwait(false),
                LandingPageUrls = landingPageUrls
            };

            return viewModel;
        }

        protected virtual List<string> GetLandingPageUrls(TreeNode<Category> startNode, GetCategoryBrowsingViewModelParam param)
        {
            var urlStack = new Stack<string>();
            var currentNode = startNode;

            while (currentNode != null && !IsCategoryFacetSystem(currentNode.Value, currentNode.GetLevel()))
            {
                var url = GetParentPageUrl(currentNode, param);
                urlStack.Push(url ?? string.Empty);

                currentNode = currentNode.Parent;
            }

            var list = new List<string>();
            while (urlStack.Count > 0)
            {
                list.Add(urlStack.Pop());
            }

            return list;
        }

        protected virtual string GetParentPageUrl(TreeNode<Category> currentNode, GetCategoryBrowsingViewModelParam param)
        {
            var nodeLevel = currentNode.GetLevel();
            var isLandingAllProducts = nodeLevel <= CategoriesConfiguration.LandingPageMaxLevel;

            string parentPageUrl;
            if (currentNode.Value.PrimaryParentCategoryId != null && currentNode.Value.PrimaryParentCategoryId != "Root")
            {
                parentPageUrl = GetCategoryUrl(currentNode.Value.PrimaryParentCategoryId, param, isLandingAllProducts);
            }
            else
            {
                parentPageUrl = GetCategoryUrl(currentNode.Value.Id, param, isLandingAllProducts);
            }

            return parentPageUrl;
        }

        protected virtual async Task<SelectedFacets> GetSelectedFacetsAsync(GetCategoryBrowsingViewModelParam param)
        {
            List<SearchFilter> selectedCategories = await GetSelectedCategoriesAsync(param).ConfigureAwait(false);
            List<SearchFilter> allFacets = selectedCategories.Concat(param.SelectedFacets).ToList();

            return FlattenFilterList(allFacets, param.CultureInfo);
        }

        protected virtual async Task<List<SearchFilter>> GetSelectedCategoriesAsync(GetCategoryBrowsingViewModelParam param)
        {
            List<Category> selectedCategories = await GetAncestorsAndSelfCategoriesAsync(param).ConfigureAwait(false);

            return selectedCategories.Select((category, i) => new SearchFilter
            {
                Name = string.Format("CategoryLevel{0}_Facet", i + 1),
                Value = category.DisplayName.GetLocalizedValue(ComposerContext.CultureInfo.Name),
                IsSystem = IsCategoryFacetSystem(category, i)
            }).ToList();
        }

        protected virtual bool IsCategoryFacetSystem(Category category, int level)
        {
            //TODO: Take from config?
            return level == 0;
        }

        protected virtual async Task<List<Category>> GetAncestorsAndSelfCategoriesAsync(GetCategoryBrowsingViewModelParam param)
        {
            TreeNode<Category> current = await GetCurrentCategoryNodeAsync(param).ConfigureAwait(false);

            var categories = new List<Category>();
            while (current != null && !current.Value.Id.Equals("Root", StringComparison.InvariantCultureIgnoreCase))
            {
                categories.Insert(0, current.Value);
                current = current.Parent;
            }

            return categories;
        }

        protected virtual async Task<TreeNode<Category>> GetCurrentCategoryNodeAsync(GetCategoryBrowsingViewModelParam param)
        {
            Tree<Category, string> tree = await CategoryRepository.GetCategoriesTreeAsync(new GetCategoriesParam
            {
                Scope = ComposerContext.Scope
            }).ConfigureAwait(false);

            if (tree.ContainsKey(param.CategoryId)) { return tree[param.CategoryId]; }

            throw new InvalidOperationException(string.Format("{0} does not exist in the retrieved category tree", param.CategoryId));
        }

        protected virtual async Task<ProductSearchResultsViewModel> GetProductSearchResultsAsync(GetCategoryBrowsingViewModelParam param)
        {
            var searchParam = new BrowsingSearchParam
            {
                Criteria = await GetSearchCriteriaAsync(param).ConfigureAwait(false),
                CategoryId = param.CategoryId,
                CategoryFilters = await GetSelectedCategoriesAsync(param).ConfigureAwait(false),
                IsAllProductsPage = param.IsAllProducts
            };

            ProductSearchResultsViewModel model = await SearchAsync(searchParam).ConfigureAwait(false);
            model.Facets = GetFacetsWithoutCategoryFacets(model.Facets);

            return model;
        }

        protected virtual async Task<SearchCriteria> GetSearchCriteriaAsync(GetCategoryBrowsingViewModelParam param)
        {
            var criteria = new CategorySearchCriteria
            {
                NumberOfItemsPerPage = SearchConfiguration.MaxItemsPerPage,
                IncludeFacets = true,
                InventoryLocationIds = param.InventoryLocationIds,
                StartingIndex = (param.Page - 1) * SearchConfiguration.MaxItemsPerPage, // The starting index is zero-based.
                SortBy = param.SortBy,
                SortDirection = param.SortDirection,
                Page = param.Page,
                BaseUrl = param.BaseUrl,
                CultureInfo = ComposerContext.CultureInfo,
                Scope = ComposerContext.Scope,
                AutoCorrect = SearchConfiguration.AutoCorrectSearchTerms,
                CategoryId = param.CategoryId,
                CategoryHasFacets = param.SelectedFacets.Any()
            };

            List<SearchFilter> selectedCategories = await GetSelectedCategoriesAsync(param).ConfigureAwait(false);
            List<SearchFilter> selectedFacets = param.SelectedFacets;

            criteria.SelectedFacets.AddRange(selectedCategories);
            criteria.SelectedFacets.AddRange(selectedFacets);

            return criteria;
        }

        protected virtual List<Facet> GetFacetsWithoutCategoryFacets(IList<Facet> facets)
        {
            return facets.Where(facet => !IsCategoryFacet(facet)).ToList();
        }

        protected virtual bool IsCategoryFacet(Facet facet)
        {
            return facet.FieldName != null && facet.FieldName.StartsWith("CategoryLevel");
        }

        protected virtual async Task<List<ChildCategoryViewModel>> GetChildCategoriesAsync(GetCategoryBrowsingViewModelParam param)
        {
            List<TreeNode<Category>> children = await GetCategoryChildrenAsync(param).ConfigureAwait(false);

            //If the category has no URL, it probably means there is no item for it in the CMS yet...
            var childCategories = children
                .Select(childCategory => CreateChildCategoryViewModel(childCategory.Value, param))
                .Where(childCategory => !string.IsNullOrWhiteSpace(childCategory.Url)).ToList();

            return childCategories;
        }

        protected virtual async Task<List<TreeNode<Category>>> GetCategoryChildrenAsync(GetCategoryBrowsingViewModelParam param)
        {
            Tree<Category, string> tree = await CategoryRepository.GetCategoriesTreeAsync(new GetCategoriesParam
            {
                Scope = ComposerContext.Scope
            }).ConfigureAwait(false);

            return tree[param.CategoryId].Children;
        }

        protected virtual ChildCategoryViewModel CreateChildCategoryViewModel(Category category, GetCategoryBrowsingViewModelParam param)
        {
            string title = category.DisplayName.GetLocalizedValue(ComposerContext.CultureInfo.Name);
            var url = GetCategoryUrl(category.Id, param);

            return new ChildCategoryViewModel { Title = title, Url = url};
        }

        protected virtual string GetCategoryUrl(string categoryId, GetCategoryBrowsingViewModelParam param, bool isAllProductsPage = false)
        {
            string url = CategoryBrowsingUrlProvider.BuildCategoryBrowsingUrl(new BuildCategoryBrowsingUrlParam
            {
                CategoryId = categoryId,
                CultureInfo = param.CultureInfo,
                BaseUrl = param.BaseUrl,
                IsAllProductsPage = isAllProductsPage
            });
            return url;
        }

        protected override string GenerateUrl(CreateSearchPaginationParam<BrowsingSearchParam> param)
        {
            var cloneParam = (BrowsingSearchParam)param.SearchParameters.Clone();

            if (cloneParam.CategoryFilters != null)
            {
                RemoveAppendedCategoryFacet(cloneParam);
            }

            var nameValueCollection = CategoryBrowsingUrlProvider.BuildSearchQueryString(new BuildSearchUrlParam()
            {
                SearchCriteria = cloneParam.Criteria
            });

            return UrlFormatter.ToUrlString(nameValueCollection);

        }

        protected static void RemoveAppendedCategoryFacet(BrowsingSearchParam cloneParam)
        {
            foreach (var filter in cloneParam.CategoryFilters)
            {
                if (filter == null) { continue; }

                var categoryFilter = cloneParam.Criteria.SelectedFacets
                    .Find(f => filter.Name == f.Name && filter.Value == f.Value);

                if (categoryFilter != null)
                {
                    cloneParam.Criteria.SelectedFacets.Remove(categoryFilter);
                }
            }
        }
    }
}