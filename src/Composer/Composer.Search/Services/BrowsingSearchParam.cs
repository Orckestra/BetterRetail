using System.Collections.Generic;
using System.Linq;
using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Search.Services
{
	public class BrowsingSearchParam : ISearchParam
	{
		public string CategoryId { get; set; }
        
		public SearchCriteria Criteria { get; set; }

		public List<SearchFilter> CategoryFilters { get; set; }

	    public bool IsAllProductsPage { get; set; }

	    public BrowsingSearchParam()
	    {
	        CategoryFilters = new List<SearchFilter>();
	    }

		public object Clone()
		{
			var clone = new BrowsingSearchParam
			{
				CategoryId = CategoryId
			};

			if (Criteria != null)
			{
				clone.Criteria = Criteria.Clone();
			}
			if (CategoryFilters != null)
			{
				clone.CategoryFilters = CategoryFilters.Select(cf => cf.Clone()).ToList();
			}

		    clone.IsAllProductsPage = IsAllProductsPage;
			return clone;
		}
	}
}