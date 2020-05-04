using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Utils;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Products;
using Orckestra.Overture.ServiceModel.Queries;
using Orckestra.Overture.ServiceModel.Requests.Products;
using Orckestra.Overture.ServiceModel.Requests.Search;
using Orckestra.Overture.ServiceModel.Search;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private const string RootCategoryId = "Root";
        private const string PropertyBagItemId = "Item_Id";
        private const string PropertyBagParentItemId = "ParentItem_Id";

        protected IOvertureClient OvertureClient { get; private set; }
        protected ICacheProvider CacheProvider { get; private set; }

        public CategoryRepository(IOvertureClient overtureClient, ICacheProvider cacheProvider)
        {
            OvertureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            CacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
        }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>List of categories.</returns>
        public async virtual Task<List<Category>> GetCategoriesAsync(GetCategoriesParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.Category)
            {
                Scope = param.Scope
            };

            var result = await CacheProvider.GetOrAddAsync(cacheKey, () => OvertureClient.SendAsync(new GetCategoriesV2Request
            {
                ScopeId = param.Scope
            }));
            return result?.Categories;
        }

        /// <summary>
        /// Gets the category tree.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<Tree<Category, string>> GetCategoriesTreeAsync(GetCategoriesParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }

            return GetCategoriesTreeAsync(param.Scope);
        }

        /// <summary>
        /// Gets the categories path from the provided categoryId to the root category.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>
        /// Categories path starting at provided category up to root category.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">param</exception>
        public async Task<List<Category>> GetCategoriesPathAsync(GetCategoriesPathParam param)
        {
            if (param == null) { throw new ArgumentNullException(nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.Scope)), nameof(param)); }
            if (string.IsNullOrWhiteSpace(param.CategoryId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(param.CategoryId)), nameof(param)); }

            var categoriesTree = await GetCategoriesTreeAsync(param.Scope).ConfigureAwait(false);

            return BuildPathFromTree(categoriesTree, param.CategoryId);
        }

        private static List<Category> BuildPathFromTree(Tree<Category, string> categoriesTree, string categoryId)
        {
            if (!categoriesTree.TryGetValue(categoryId, out TreeNode<Category> categoryNode))
            {
                throw new ArgumentException("categoryId doesn't exist", "categoryId");
            }

            var path = new List<Category>
            {
                categoryNode.Value
            };

            while (categoryNode.HasParent)
            {
                categoryNode = categoryNode.Parent;
                path.Add(categoryNode.Value);
            }

            return path;
        }

        private async Task<Tree<Category, string>> GetCategoriesTreeAsync(string scope)
        {
            var categories = await GetCategoriesAsync(new GetCategoriesParam
            {
                Scope = scope
            }).ConfigureAwait(false);

            // TODO: Remove that method once OCS provide correct value for PrimaryParentCategoryId
            // TODO: (SB does OCS return the correct value now? 02/06/15)
            FixMissingPrimaryParentCategoryIds(categories);

            return new Tree<Category, string>(categories, category => category.Id, category => category.PrimaryParentCategoryId, StringComparer.InvariantCultureIgnoreCase);
        }

        private void FixMissingPrimaryParentCategoryIds(List<Category> categories)
        {
            // Create dictionary based on category inner id
            var categoriesIdLookup = new Dictionary<int, Category>();
            foreach (var category in categories)
            {
                if (category.PropertyBag.TryGetValue(PropertyBagItemId, out object itemId))
                {
                    categoriesIdLookup[(int)itemId] = category;
                }
            }

            // Set each category's PrimaryParentCategoryId based on inner ids
            foreach (var category in categories)
            {
                if (category.PropertyBag.TryGetValue(PropertyBagParentItemId, out object parentCategoryId))
                {
                    var parentCategory = categoriesIdLookup[(int)parentCategoryId];
                    category.PrimaryParentCategoryId = parentCategory.Id;
                }
            }
        }

        public virtual Task<List<Facet>> GetCategoryProductCount(string scopeId, string cultureName)
        {
            var request = GetProductsAdvancedSearchRequest(scopeId, cultureName, "CategoryAutoSuggest");
            return Task.FromResult(OvertureClient.Send(request).Facets);
        }

        public virtual Task<List<Facet>> GetBrandProductCount(string scopeId, string cultureName)
        {
            var request = GetProductsAdvancedSearchRequest(scopeId, cultureName, "BrandAutoSuggest");
            return Task.FromResult(OvertureClient.Send(request).Facets);
        }

        protected virtual AdvancedSearchRequest GetProductsAdvancedSearchRequest(string scopeId, string cultureName, string facetHierarchyId)
        {
            return new AdvancedSearchRequest
            {
                CultureName = cultureName,
                IndexName = "Products",
                ScopeId = scopeId,
                SearchTerms = "*",
                IncludeFacets = true,
                FacetHierarchyId = facetHierarchyId,
                Query = new Query
                {
                    MaximumItems = 0,
                    Filter = new FilterGroup
                    {
                        BinaryOperator = BinaryOperator.And,
                        Filters = new List<Filter>
                        {
                            new Filter
                            {
                                Member = "CatalogId",
                                Operator = Operator.Equals,
                                Value = scopeId
                            },
                            new Filter
                            {
                                Member = "Active",
                                Operator = Operator.Equals,
                                Value = true
                            }
                        }
                    }
                }
            };
        }
    }
}
