using System.Collections.Generic;
using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Search.Services
{
	public sealed class BrowsingByCategoryParam : ISearchParam
	{
		public string CategoryId { get; set; }

		public SearchCriteria Criteria { get; set; }

	    public string CategoryName { get; set; }

	    public bool IsAllProducts { get; set; }

        public List<string> InventoryLocationIds { get; set; }

	    public BrowsingByCategoryParam()
	    {
	        InventoryLocationIds = new List<string>();
	    }

		public object Clone()
		{
			var clone = (BrowsingByCategoryParam) (MemberwiseClone());
			
            if (Criteria != null)
			{
				clone.Criteria = Criteria.Clone();
			}

		    clone.CategoryName = CategoryName;
		    clone.IsAllProducts = IsAllProducts;
            clone.InventoryLocationIds = new List<string>(InventoryLocationIds);

			return clone;
		}
	}
}
