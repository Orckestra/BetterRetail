using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Configuration;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Providers;
using Orckestra.Composer.Utils;
using Orckestra.Overture;
using Orckestra.Overture.Caching;
using Orckestra.Overture.ServiceModel.Products;
using Orckestra.Overture.ServiceModel.Requests.Products;

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
            if (overtureClient == null) { throw new ArgumentNullException("overtureClient"); }
            if (cacheProvider == null) { throw new ArgumentNullException("cacheProvider"); }

            OvertureClient = overtureClient;
            CacheProvider = cacheProvider;
        }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <returns>List of categories.</returns>
        public Task<List<Category>> GetCategoriesAsync(GetCategoriesParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), "param"); }

            var cacheKey = new CacheKey(CacheConfigurationCategoryNames.Category)
            {
                Scope = param.Scope
            };

            return CacheProvider.GetOrAddAsync(cacheKey, () => OvertureClient.SendAsync(new GetCategoriesRequest
            {
                ScopeId = param.Scope
            }));
        }

        /// <summary>
        /// Gets the category tree.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task<Tree<Category, string>> GetCategoriesTreeAsync(GetCategoriesParam param)
        {
            if (param == null) { throw new ArgumentNullException("param"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException("param.Scope may not be null or whitespace", "param"); }

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
            if (param == null) { throw new ArgumentNullException("param"); }
            if (string.IsNullOrWhiteSpace(param.Scope)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("Scope"), "param"); }
            if (string.IsNullOrWhiteSpace(param.CategoryId)) { throw new ArgumentException(ArgumentNullMessageFormatter.FormatErrorMessage("CategoryId"), "param"); }

            var categoriesTree = await GetCategoriesTreeAsync(param.Scope).ConfigureAwait(false);

            return BuildPathFromTree(categoriesTree, param.CategoryId);
        }

        private static List<Category> BuildPathFromTree(Tree<Category, string> categoriesTree, string categoryId)
        {
            TreeNode<Category> categoryNode;
            if (!categoriesTree.TryGetValue(categoryId, out categoryNode))
            {
                throw new ArgumentException("categoryId doesn't exist", "categoryId");
            }

            var path = new List<Category>();

            path.Add(categoryNode.Value);
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
                object itemId;
                if (category.PropertyBag.TryGetValue(PropertyBagItemId, out itemId))
                {
                    categoriesIdLookup[(int)itemId] = category;
                }
            }

            // Set each category's PrimaryParentCategoryId based on inner ids
            foreach (var category in categories)
            {
                object parentCategoryId;
                if (category.PropertyBag.TryGetValue(PropertyBagParentItemId, out parentCategoryId))
                {
                    var parentCategory = categoriesIdLookup[(int)parentCategoryId];
                    category.PrimaryParentCategoryId = parentCategory.Id;
                }
            }
        }
    }
}
