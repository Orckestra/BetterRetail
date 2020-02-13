using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Repositories;
using Orckestra.Composer.Search.Facets;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Composer.Search.ViewModels;
using Orckestra.Composer.Services;
using Orckestra.Composer.Utils;
using Orckestra.Composer.ViewModels;
using Orckestra.Overture.ServiceModel.Products;

namespace Orckestra.Composer.Search.Services
{
    /// <summary>
    /// Service use to retrieves the products from the browsing pages.
    /// </summary>
    public class CategoryBrowsingViewService2 : BaseSearchViewService<BrowsingSearchParam>, ICategoryBrowsingViewService2
    {
        private readonly IComposerContext _context;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryBrowsingUrlProvider _categoryBrowsingUrlProvider;

        public CategoryBrowsingViewService2(ISearchRepository searchRepository,
                                            IViewModelMapper viewModelMapper,
                                            IDamProvider damProvider,
                                            ILocalizationProvider localizationProvider,
                                            IProductUrlProvider productUrlProvider,
                                            ISearchUrlProvider searchUrlProvider,
                                            IComposerContext context,
                                            ICategoryRepository categoryRepository,
                                            ICategoryBrowsingUrlProvider categoryBrowsingUrlProvider) : base(searchRepository, viewModelMapper, damProvider, localizationProvider, productUrlProvider, searchUrlProvider)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            if (categoryRepository == null) { throw new ArgumentNullException("categoryRepository"); }

            _context = context;
            _categoryRepository = categoryRepository;
            _categoryBrowsingUrlProvider = categoryBrowsingUrlProvider;
        }

        public async Task<CategoryBrowsingViewModel> GetCategoryBrowsingViewModelAsync(GetCategoryBrowsingViewModelParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (param.CategoryId == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CategoryId")); }
            if (param.SelectedFacets == null) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("SelectedFacets")); }

            var viewModel = new CategoryBrowsingViewModel
            {
                CategoryId = param.CategoryId,
                CategoryName = param.CategoryName,
                SelectedFacets = await GetSelectedFacetsAsync(param).ConfigureAwait(false),
                ProductSearchResults = await GetProductSearchResultsAsync(param).ConfigureAwait(false),
                ChildCategories = await GetChildCategoriesAsync(param).ConfigureAwait(false)
            };

            viewModel.Context["ProductSearchResults"] = viewModel.ProductSearchResults;
            viewModel.Context["ListName"] = "Category Browsing";

            return viewModel;
        }

        protected virtual async Task<SelectedFacets> GetSelectedFacetsAsync(GetCategoryBrowsingViewModelParam param)
        {
            List<SearchFilter> selectedCategories = await GetSelectedCategoriesAsync(param).ConfigureAwait(false);
            List<SearchFilter> selectedFacets = param.SelectedFacets;

            List<SearchFilter> allFacets = selectedCategories.Concat(selectedFacets).ToList();

            return BuildSelectedFacets(allFacets);
        }

        private async Task<List<SearchFilter>> GetSelectedCategoriesAsync(GetCategoryBrowsingViewModelParam param)
        {
            List<Category> selectedCategories = await GetAncestorsAndSelfCategoriesAsync(param).ConfigureAwait(false);

            return selectedCategories.Select((category, i) => new SearchFilter
            {
                Name = String.Format("CategoryLevel{0}_Facet", i + 1),
                Value = category.DisplayName.GetLocalizedValue(_context.CultureInfo.Name),
                IsSystem = true

            }).ToList();
        }

        private async Task<List<Category>> GetAncestorsAndSelfCategoriesAsync(GetCategoryBrowsingViewModelParam param)
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

        private async Task<TreeNode<Category>> GetCurrentCategoryNodeAsync(GetCategoryBrowsingViewModelParam param)
        {
            Tree<Category, string> tree = await _categoryRepository.GetCategoriesTreeAsync(new GetCategoriesParam { Scope = _context.Scope }).ConfigureAwait(false);

            if (tree.ContainsKey(param.CategoryId))
            {
                return tree[param.CategoryId];
            }

            throw new InvalidOperationException(String.Format("{0} does not exist in the retrieved category tree", param.CategoryId));
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

        private async Task<SearchCriteria> GetSearchCriteriaAsync(GetCategoryBrowsingViewModelParam param)
        {
            var criteria = new SearchCriteria
            {
                NumberOfItemsPerPage = SearchConfiguration.MaxItemsPerPage,
                IncludeFacets = true,
                StartingIndex = (param.Page - 1) * SearchConfiguration.MaxItemsPerPage, // The starting index is zero-based.
                SortBy = param.SortBy,
                SortDirection = param.SortDirection,
                Page = param.Page,
                BaseUrl = param.BaseUrl,
                CultureInfo = _context.CultureInfo,
                Scope = _context.Scope
            };

            List<SearchFilter> selectedCategories = await GetSelectedCategoriesAsync(param).ConfigureAwait(false);
            List<SearchFilter> selectedFacets = param.SelectedFacets;

            criteria.SelectedFacets.AddRange(selectedCategories);
            criteria.SelectedFacets.AddRange(selectedFacets);

            return criteria;
        }

        private List<Facet> GetFacetsWithoutCategoryFacets(IList<Facet> facets)
        {
            return facets.Where(facet => !IsCategoryFacet(facet)).ToList();
        }

        private bool IsCategoryFacet(Facet facet)
        {
            return facet.FieldName != null && facet.FieldName.StartsWith("CategoryLevel");
        }

        protected virtual async Task<List<ChildCategoryViewModel>> GetChildCategoriesAsync(GetCategoryBrowsingViewModelParam param)
        {
            List<TreeNode<Category>> children = await GetCategoryChildrenAsync(param).ConfigureAwait(false);
            return children.Select(childCategory => CreateChildCategoryViewModel(childCategory.Value, param)).ToList();
        }

        private async Task<List<TreeNode<Category>>> GetCategoryChildrenAsync(GetCategoryBrowsingViewModelParam param)
        {
            Tree<Category, string> tree = await _categoryRepository.GetCategoriesTreeAsync(new GetCategoriesParam { Scope = _context.Scope }).ConfigureAwait(false);
            return tree[param.CategoryId].Children;
        }

        private ChildCategoryViewModel CreateChildCategoryViewModel(Category category, GetCategoryBrowsingViewModelParam param)
        {
            string title = category.DisplayName.GetLocalizedValue(_context.CultureInfo.Name);
            string url = _categoryBrowsingUrlProvider.BuildCategoryBrowsingUrl(new BuildCategoryBrowsingUrlParam
            {
                CategoryId = category.Id,
                CultureInfo = _context.CultureInfo,
                BaseUrl = param.BaseUrl,
                IsAllProductsPage = false
            });

            return new ChildCategoryViewModel { Title = title, Url = url };
        }

        protected override string GenerateUrl(BrowsingSearchParam byCategoryCriteria)
        {
            var cloneParam = (BrowsingSearchParam)byCategoryCriteria.Clone();

            if (cloneParam.CategoryFilters != null)
            {
                RemoveAppendedCategoryFacet(cloneParam);
            }
            var param = new BuildCategoryBrowsingUrlParam
            {
                CategoryId = byCategoryCriteria.CategoryId,
                Criteria = cloneParam.Criteria,
                IsAllProductsPage = byCategoryCriteria.IsAllProductsPage
            };

            return _categoryBrowsingUrlProvider.BuildCategoryBrowsingUrl(param);
        }

        private static void RemoveAppendedCategoryFacet(BrowsingSearchParam cloneParam)
        {
            foreach (var filter in cloneParam.CategoryFilters)
            {
                if (filter == null)
                {
                    continue;
                }
                var categoryFilter =
                    cloneParam.Criteria.SelectedFacets.FirstOrDefault(f => filter.Name == f.Name && filter.Value == f.Value);
                if (categoryFilter != null)
                {
                    cloneParam.Criteria.SelectedFacets.Remove(categoryFilter);
                }
            }
        }
    }
}
