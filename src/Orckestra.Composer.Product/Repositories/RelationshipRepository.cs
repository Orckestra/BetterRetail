using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orckestra.Composer.Product.Parameters;
using Orckestra.Composer.Search.Repositories;
using Orckestra.Overture;
using Orckestra.Overture.ServiceModel.Queries;
using Orckestra.Overture.ServiceModel.Requests.Search;
using Orckestra.Overture.ServiceModel.Search;

namespace Orckestra.Composer.Product.Repositories
{
    public class RelationshipRepository : IRelationshipRepository
    {
        protected readonly IOvertureClient _overtureClient;
        protected readonly IProductRequestFactory _productRequestFactory;

        public RelationshipRepository(IOvertureClient overtureClient, IProductRequestFactory productRequestFactory)
        {
            _overtureClient = overtureClient ?? throw new ArgumentNullException(nameof(overtureClient));
            _productRequestFactory = productRequestFactory ?? throw new ArgumentNullException(nameof(productRequestFactory));
        }

        public virtual Task<ProductSearchResult> GetProductInSameCategoryAsync(GetProductsInSameCategoryParam getProductsInSameCategoryParam)
        {
            var request = CreateProductInSameCategorySearchRequest(getProductsInSameCategoryParam);
            return _overtureClient.SendAsync(request);
        }

        protected virtual SearchAvailableProductsRequest CreateProductInSameCategorySearchRequest(
            GetProductsInSameCategoryParam getProductsInSameCategoryParam)
        {
            var request = _productRequestFactory.CreateProductRequest(getProductsInSameCategoryParam.Scope);

            request.Query.IncludeTotalCount = false;
            request.Query.MaximumItems = getProductsInSameCategoryParam.MaxItems;
            request.CultureName = getProductsInSameCategoryParam.CultureInfo.Name;
            request.InventoryLocationIds = getProductsInSameCategoryParam.InventoryLocationIds;
            request.ScopeId = getProductsInSameCategoryParam.Scope;

            var sortDefinitions = BuildQuerySortings(getProductsInSameCategoryParam.SortBy, getProductsInSameCategoryParam.SortDirection);

            if (sortDefinitions != null)
            {
                request.Query.Sortings.Add(sortDefinitions);
            }

            var filters = new List<Filter>()
            {
                new Filter()
                {
                    Member = "ParentCategoryId",
                    Value = getProductsInSameCategoryParam.CategoryId
                }
            };

            if (!string.IsNullOrWhiteSpace(getProductsInSameCategoryParam.CurrentProductId))
            {
                filters.Add(new Filter()
                {
                    Member = "ProductId",
                    Value = getProductsInSameCategoryParam.CurrentProductId,
                    Not = true
                });
            }

            request.Query.Filter.FilterGroups.Add(new FilterGroup()
            {
                Filters = filters
            });
            return request;
        }

        protected virtual QuerySorting BuildQuerySortings(string sortBy, string sortDirection)
        {
            if (string.IsNullOrWhiteSpace(sortBy)) { return null; }

            var sortDirectionEnum =
                string.IsNullOrWhiteSpace(sortDirection) ||
                sortDirection.Equals("asc", StringComparison.InvariantCultureIgnoreCase)
                ? SortDirection.Ascending
                : SortDirection.Descending;

            return new QuerySorting
            {
                Direction = sortDirectionEnum,
                PropertyName = sortBy
            };
        }
    }
}