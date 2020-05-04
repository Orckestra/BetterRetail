using System;
using System.Collections.Generic;
using Orckestra.Composer.Parameters;
using Orckestra.Composer.Search.Parameters;
using Orckestra.Overture.ServiceModel.Queries;
using Orckestra.Overture.ServiceModel.Requests.Search;
using static Orckestra.Composer.Utils.MessagesHelper.ArgumentException;

namespace Orckestra.Composer.Search.Repositories
{
    public class ProductRequestFactory : IProductRequestFactory
    {
        /// <summary>
        /// Creates the query to see which products are active for a given catalog, i.e. scope.
        /// </summary>
        /// <param name="scopeId">The scope identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">scope id cannot be null or empty;scopeId</exception>
        public virtual SearchAvailableProductsRequest CreateProductRequest(string scopeId)
        {
            if (string.IsNullOrWhiteSpace(scopeId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(), nameof(scopeId)); }

            var request = new SearchAvailableProductsRequest
            {
                Query = CreateQuery(scopeId)
            };

            return request;
        }

        /// <summary>
        /// Creates the query to see which products are active for a given catalog, i.e. scope.
        /// </summary>
        /// <param name="criteria">Search criteria.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">criteria cannot be null;criteria</exception>
        /// <exception cref="System.ArgumentException">criteria.Scope cannot be null or empty;criteria.Scope</exception>
        public virtual SearchAvailableProductsBaseRequest CreateProductRequest(SearchCriteria criteria)
        {
            if (criteria == null) { throw new ArgumentNullException(nameof(criteria)); }
            if (string.IsNullOrWhiteSpace(criteria.Scope)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(nameof(criteria.Scope)), nameof(criteria)); }

            if (criteria is CategorySearchCriteria categoryCriteria && !categoryCriteria.CategoryHasFacets)
            {
                return new SearchAvailableProductsByCategoryRequest()
                {
                    Query = CreateQuery(criteria.Scope),
                    CategoryName = categoryCriteria.CategoryId
                };
            }
            return CreateProductRequest(criteria.Scope);
        }

        protected virtual Query CreateQuery(string scopeId)
        {
            if (string.IsNullOrWhiteSpace(scopeId)) { throw new ArgumentException(GetMessageOfNullWhiteSpace(), nameof(scopeId)); }

            return new Query
            {
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
                                Value= bool.TrueString
                            }
                        }
                }
            };
        }
    }
}
