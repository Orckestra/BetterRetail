using Orckestra.Composer.Parameters;

namespace Orckestra.Composer.Search.Services
{
	public sealed class SearchParam : ISearchParam
	{
		public SearchCriteria Criteria
		{
			get; set;
		}

		public object Clone()
		{
			return new SearchParam { Criteria = Criteria};
		}
	}
}